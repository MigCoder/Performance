# 程序性能优化 
language: C#, JavaScript 


###一. CPU Cache Hitting#

A、B 两段相似代码，你觉得哪一段的速度会快一点？或者一样快？

初始化：

	int width, height;
	width = height = 4000;
	var arr = new int[width, height];
A：
		
	for (int x = 0; x < width; x++)
	{
		// 按行遍历
	    for (int y = 0; y < height; y++)
	    {
	        arr[x, y] = 0;
	    }
	}
B：

	for (int y = 0; y < height; y++)
	{
		// 按列遍历
	    for (int x = 0; x < width; x++)
	    {
	        arr[x, y] = 0;
	    }
	}


.

.

.

.

.

.

.

.

.

通常的结果是 B 比 A 慢差不多一倍左右，下边是我的实验结果：

A:

![](https://github.com/ilovezcd/Performance/blob/master/Images/1.jpg)

B:

![](https://github.com/ilovezcd/Performance/blob/master/Images/2.jpg)

导致这么大差异的问题在于 A 版本的执行过程中 CPU 缓存一直命中率低，现代 CPU 在获取内存中数据时，都是以块为单位，而不是以字节。

CPU 会尝试预测代码想要访问的数据，以块为单位将其预先加载到 CPU 各级缓存中（L1~L3），一个很简单的例子当你访问数组首个元素的时候，CPU 可能已经将头 8 个元素当作一个块（块的大小由操作系统和 CPU 架构决定）加载进 CPU 缓存了，那么你访问下一个元素的时候速度就会快。

上边的例子中 A 版本的代码会导致 CPU 的这种预测一直失效，几乎每次都需要丢弃已经缓存的数据，而 B 版本的代码每一次都能极高几率的命中缓存。

###二.Struct and boxing

####2.1.1 值类型的定义
所有的值类型均派生自 **System.ValueType**,但不是所有派生自 **System.ValueType** 的类型都是值类型，例如 Enum 类就不是值类型。

####2.1.2 值类型、引用类型的内存结构区别

引用类型：

![](https://github.com/ilovezcd/Performance/blob/master/Images/4.jpg)

值类型：

![](https://github.com/ilovezcd/Performance/blob/master/Images/5.jpg)

可以看出引用类型比值类型多出了两个字段：

Object Header Word、Method Table Pointer

Object Header Word 的用处比较多，例如线程同步，类型信息指针等。而 Method Table Pointer是一张方法表的指针，指向引用类型继承、重写、自有的方法在内存中的地址列表。

值类型是没有上边两个字段的，只是保存着一些数值而已，那么是不是值类型就不能调用方法了呢？

####2.1.3 装箱：使值类型能调用方法

装箱能够赋予值类型以上两个字段，也就可以调用从 **System.ValueType** 继承来的方法了：

![](https://github.com/ilovezcd/Performance/blob/master/Images/6.jpg)

注意装箱后值类型实例本身没有消失，而是被**复制**了一份到“箱子”中，所以值类型装箱会导致性能下降，当值类型的字段越多的时候性能下降越明显。
####2.1.4 使用 IL constrained 指令避免装箱
理论上来讲值类型实例只要一调用方法，它就会被装箱以便调用方法表，但是 .Net 编译器提供了 constrained 指令，它的作用是允许 callvirt 指令以一致的方式来调用引用类型和值类型，也就是会确定好实际方法表的位置。
它的原则是这样的， 其中 thisType 指的是实例的类型。：

- If **thisType** is a reference type (as opposed to a value type) then ptr is dereferenced and passed as the 'this' pointer to the callvirt of method.
- If **thisType** is a value type and thisType **implements method** then ptr is passed unmodified as the 'this' pointer to a call  method instruction, for the implementation of method by thisType.
- If **thisType **is a value type and thisType **does not implement method** then ptr is dereferenced, boxed, and passed as the 'this' pointer to the callvirt  method instruction.

所以会发生装箱的只有一种情况，thisType 为值类型，且**没有重写**从 ValueType 继承而来的几个方法（ToString、Equal、GetHashCode），值类型自有的方法以及实现接口的方法不会发生装箱。

实际上你可以将 constrained 理解为获取**正确方法表地址**的指令，我们的目的只是要获取一个 Method Table Pointer，好让我们把值类型实例传过去给正确的方法执行，而装箱就多做了一步复制数据的操作。 

还有一种装箱情况是将值类型转换为接口对象，因为根据定义接口也是引用类型，它不继承自 **System.ValueType** 本身也可以继承其他接口，所以在我们显示实现值类型的接口的时候要注意防止装箱。

####2.2 堆和栈
堆和栈其实并无本质区别，只是 CLR 在程序运行时将进程内存分割的两个区域而已，局部变量、值类型实例会被分配到栈，引用对象会被分配到堆。

栈在每个方法返回的时候都会强制 GC 回收一下，以便删除所有局部变量，整理栈内存。

而堆则由 GC 根据自己算法去管理，堆自身还划分为**托管堆**和**非托管堆**，**非托管堆**是专门拿来放置大对象的，因为 GC 回收对象的时候除了删除数据，它还会整理数据的位置，消灭内存碎片，然而一个大对象被删除后，将其他数据整理位置的成本会非常大，所以 GC 基本上不整理**非托管堆**。

因为栈比较小，而且基本没有内存碎片，所以在它上边分配数组并进行访问的速度要比堆上来的快，不过一般栈的大小默认为1MB，只能存差不多25万个 int 数据。

在栈上分配数据需要 unsafe 上下文，并且使用 stackalloc 关键字，其返回值是一个指针，意味着你得自己控制访问边界：

	private unsafe static void Alloc_Stack(int size)
	{
	    var arr = stackalloc int[size];
	    for (int i = 0; i < size; i++)
	    {
	        *arr = i;
	        arr++;
	    }
	}
这是性能对比，其实提升不是很明显：

![](https://github.com/ilovezcd/Performance/blob/master/Images/10.jpg)

![](https://github.com/ilovezcd/Performance/blob/master/Images/11.jpg)

###三. I/O Performance#
.Net Framework 提供了很方便的文件管理类库，主要的类是 FileStream 文件流，使用它有几个点需要特别注意。

####3.1 缓存 I/O 与无缓存 I/O。
当我们调用 FileStream.Read 读取数据的时候，其实不是简单的打开文件读取字节这么简单，中间要经过 Windows File Cache> HostBus Cache> Disk Cache 总共三层缓存，我们能干预的只有 Windows File Cache。

![](https://github.com/ilovezcd/Performance/blob/master/Images/3.png)

Window File Cache 缓存存在的目的是减少硬盘读写命令的调用次数，如果每次读取文件都是一个字节，而又禁用了 Windows File Cache，那就意味着每个字节都需要发出一次实际的硬盘读写命令，操作系统在用户代码和内核代码中不停切换，性能急剧下降。

而如果启用了 Window File Cache ，那么操作系统会像 CPU 一样一直帮你预读一些数据到内存中（数据大小 = 缓存区大小），后续的每个字节读取对我们来说只是内存到内存的拷贝，也就类似过马路时一群人闯红灯总比一个接着一个闯红灯效率来得高。

那么缓存区的大小应该多大比较合适？，在创建 FileStream 的时候不指定缓存它就会帮你自动选择合适的大小,通常是 CPU L1 Cache 缓存的大小（16KB、32KB是主流），因为无论如何你的数据都是要经过 CPU 的，只有这种情况下 L1 Cache 才能一次整整读一段数据，这样能取得最平衡的性能，但不是最快的。

你可以多尝试将其设置为 L1 Cache 的倍数，提高一些内存占用率，从而更快的处理文件,64KB 就是不错的选择，当然还是需要实际测量一下。 

但如果你不是一个字节一个字节来读取文件，而是希望尽快处理大文件，那么无缓存 I/O 非常适合采用。

无缓存不是真的没有缓存，只是禁用了 Windows File Cache，省去了维护这个 Cache 的成本。无缓存 I/O 每次读写的数据量必须为磁盘扇区的大小或其倍数，不然无法获得性能提升。

总结：

 - 小文件大量**小数据块读写**，将文件缓存配置为 CPU 缓存大小或其倍数即可。
 - 大文件大量**大数据块读写**，使用无缓存 I/O ，同样将缓存大小设置为 CPU 缓存大小或其倍数。
 - 关于无缓存 I/O 的具体实现可以看我附件里的文档。

####3.2 配置 FileOptions。

FileOptions.SequentialScan
FileOptions.RandomAccess
以上两个标志都用于暗示操作系统，我这个 FileStream 是要按顺序访问，从头读到尾的，还是随机访问，一会读这一会读那的。指定后操作系统可以更好的优化文件缓存策略。


####3.3 创建 FileStream 后调用 SetLength。
调用 SetLength 的意义不在于设置一个属性来代表文件大小，而是它发出了一个通知，告知操作系统我这个 FileStream 大概需要多少字节来存放数据，操作系统会来**尽量**查找一个足够大的连续的磁盘空间来存放文件，即使没有这么大的连续空间，它也可以最优化的减少**磁盘碎片**。

而如果你不设定 Length，操作系统无从得知你大概会写多少数据，它只能走着瞧，可想而知你的数据大部分时候会是分散在**磁盘碎片**里的。

虽然 SSD 硬盘的性能不会受到磁盘碎片的影响，但如果提前通知文件的大小应该还是有优化的。

####3.4 使用异步 I/O
在将 FileOption 配置为 FileOptions.Asynchronous 后可以启用异步 I/O，调用 BeginRead 等 API 可以不再阻塞线程，适合服务器程序使用。

###四. Parallel And OpenCL

####4.1 避免使用原生 Thread
.Net 自带的线程池、Task 类库、PLinq 等多线程技术，大部分时候都比你自己管理 Thread 要好，除非你实现了自己的线程池机制，否则尽量使用它们。

####4.2 用 OpenCl 加速你的程序

如果你的算法已经足够高效，没有什么可以优化的地方，你可以考虑将它拆分成并行算法，使用 OpenCL 将其运行在 GPU 等设备，成倍的提升运算速度。

OpenCL 是用于利用多核设备并行计算的一套框架，提供了一套语法去编写在OpenCL设备上运行的代码，也就是说我们可以利用它来并行我们的程序算法。

OpenCL 设备不局限于 GPU，只要是实现其规定的 API 设备都可以作为 OpenCL 设备，现在有很多 CPU 也支持 OpenCL，当它们运行在 OpenCL 模式下时，相应的性能也会提升（类似模式切换）。

OpenCL 的一个 Demo 示例在 Github 目录里，感兴趣可以下载来看看。

####4.3 OpenCL、OpenGL、DirectX

OpenCL 全称是 Open Computing Language，是一门纯粹为计算而指定的语言（基于 C 语言 C99 标准），其内置的库函数基本都是数学库，没有太多图像相关的东西。

OpenGL 全称是 Open Graphics Library, 是一个图像编程库，与 DirectX 类似，两者都提供了大量数学、图像、游戏方面的库函数，其在底层也有类似 OpenCL 之类的语言能将代码编译并运行到 GPU 设备。

###五.IIS File Cache
IIS 支持 User 模式缓存与 Kernel 模式缓存，都只是缓存指定后缀名的文件，区别就是 Kernel 模式的缓存不允许 Url 有查询字符串，User 模式允许。
配置步骤如下：

![](https://github.com/ilovezcd/Performance/blob/master/Images/7.jpg)

![](https://github.com/ilovezcd/Performance/blob/master/Images/8.jpg)

![](https://github.com/ilovezcd/Performance/blob/master/Images/9.jpg)


###六.Angular Js

####6.1 {{ value }} vs {{ ::value }}
默认的 Angular 属性绑定只有 **Model 更新 UI** 以及 **Model UI 双向更新** 两种模式，AngularJs 1.3 版本提供了一个新的绑定语法 **{{ ::value }}**, 可以只读取一次 Model 的值到 UI，后续所有 Model 的更新都不再检测，提升了性能。

####6.2 $scope.$digest() vs $scope.$apply()

**$scope.$apply()** 会调用 $scope.$root.$digest(), 也就是从 $rootScope 开始对所有 Scope 进行 digest, 而 **$scope.$digest**() 只是从指定的 $scope 开始对它以及子 Scope 进行 digest，所以大部分情况下我们应该优先选择 **$scope.$digest()**。

另外大多数时候你只需要使用 $timeOut 将更新的语句包含进 AngularJs 的执行上下文，即可避免这两个 API 的调用，它使用 try/catch 块包装要执行的函数，能避免引发 **'a digest cycle is already in progress'** 这种异常，最后根据你指定的参数确认是否调用 **$scope.$apply()**(默认为 true)。

####6.3 Use $filter
Angular 的过滤器除了可以写在 HTML 里面，还可以使用 JavaScprit 调用 $filter 服务，写在 HTML 里的 filter 通常会被调用两次，而使用 $filter 服务则是预处理一次，此时 HTML 里面已经没有 filter 指令，所以 filter 不会再被调用。


####6.4 use 'ng-repeat' with 'track by'
ng-repeat 指令生成的为每个 Item 指定了一个 $$hashkey 属性，用来标识 Item，如果你在后台将 Item 所属的集合替换了，即使有些 Item 在你看来根本没变（例如搜索过滤的时候，符合条件的 Item 在集合中是未变的），但是 ng-repeat 依旧认为整个集合都变了，导致它删除所有 Item 项的 Dom 节点重新生成重新编译，因为你在替换集合的时候丢失了 $$hashkey 属性。

避免重复创建的办法就是将 Item 的标识属性配置到 ng-repeat 中，使用 'item in items **track by item.Id**' 替代 'item in items'。
