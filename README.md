官方文档： https://www.consul.io

    关于Consul（https://www.consul.io）是一个分布式，高可用，支持多数据中心的服务发现和配置共享的服务软件，由harshicorp公司用Go语言开发，基于Mozilla public License 2.0的协议进行开源。在Consul的文档上，Consul支持Service Discovery，Health Checking, Key/Value Store， Muti DataCenter。运用Consul，可以在系统中build复杂的应用和服务的发现等。
     Ocelot对Consul支持是天生集成，在OcelotGateway项目中configuration.json配置就可以开启consul+ocelot的使用。
    Consul的功能包括：服务注册，服务发现，API网关，负载均衡，限流，容错告警，弹性和瞬态故障处理。
  
    Consul agent有两种，一种是Server，一种是client, 官方建议Server端达到3台以上才高可用，但不要太多，太多会给集群间数据同步造成压力，client数量不限。
    
     多个server端之间会选择出一个leader，当一个server的leader宕机则会从其他server端 “投票”选择新leader。

    Consul启动有两种方式，一种是命令行，一种是配置文件的方式、
     命令行启动一个consul的server端
    consul agent -server -ui -bootstrap-expect 2 -data-dir opt/consul/data -node servermaster -bind 10.2.114.23 client 0.0.0.0
     
     关键参数说明：
      -server : 定义agent运行server模式，每个集群至少一个server，建议集群的server不要超过5个。//server模式启动
      -ui: 开启ui界面（consul.exe内部带了GUI图形界面操作）
     -bootstrap: 用来控制一个server是否在bootstrap模式，在一个datacenter中只能有一个server处于bootstrap模式，当一个server处于bootstrap模式
      -bootstrap-expect 2：在一个datacenter中期望提供的server节点数目，当该值提供的时候，consul一直等到达到执行server数目的时候才会引导整个集群。
      -data-dir： 提供一个用来存放agent状态，所有的agent允许都需要该目录，该目录必须是稳定的，系统重启后都继续存在。//consul产生文件路径（consul自己会产生一个数据存储位置）
       -node： 节点在集群中的名称，在一个集群中必须是唯一的，默认是该节点的主机名。//此节点名称
       -bind: 该地址用来在集群内部的通讯，集群内的所有节点到地址都必须是可达的，默认是0.0.0.0//集群内部通讯地址， 默认 0.0.0.0
       -config-dir:  提供一个文件目录，里面所有以json结尾的文件都会被加载。
       -config-file:  明确指定要加载哪个文件
       -advertise: 通知展现地址用来改变我们给集群中的其他节点展现的地址，一般情况下-bind地址就是展现地址
      -client: consul绑定在哪个client地址上，这个地址提供HTTP，DNS，RPC等服务，默认是127.0.0.1
      -dc: 该标记控制agent允许的datacenter的名称，默认是dc1
      -encrypt：指定secret key， 使consul在通讯时进行加密，key可以通过consul keygen生成，同一个集群中的节点必须使用相同的key。
     -join： 加入一个已经启动的agent的ip地址，可以多次指定多个agent的地址。如果consul不能加入任何指定的地址中，则agent会启动失败，
     -retry-join： 和join类似，但是允许你在第一次失败后进行尝试
     -retry-interval：两次join之间的时间间隔，默认是30s
     -retry-max： 尝试重复join的次数，默认是0，也即是无限次尝试
     -log-level: consul agent启动后显示的日志信息级别，默认是info，可选：trace，debug，info，warn，err。
        

*******搭建consul +Ocelt
https://www.cnblogs.com/xiaoliangge/p/10221950.html

******搭建Consul集群错误合集：
https://blog.csdn.net/qq_38659629/article/details/82804449

问题记录：

1、Failed to connect the 10.2.23.45 because the target mechine refuse connect

    【solved】将8300,8301,8302,8500,8600加入到防火墙允许port列表

2、Failed to join 10.2.23.45: Member 'consul_server1' has conflicting node ID 'b76ff298-accd-05ff-8c64-5d79d866dfa9' with this agent's ID

     【solved】1）、在命令启动agent时加入-disable-host-node-id参数，禁止生成node-id，类似这样：$ consul agent -server -ui -bootstrap-expect=2 -data-dir=/data -node consul_server02 -client=0.0.0.0 -bind=10.2.118.50 -disable-host-node-id
                     2)、可以用-node-id生成一个新的node-id ,类似这样 $ consul agent -server -ui -bootstrap-expect=2 -data-dir=/data -node=consul_server02 -client=0.0.0.0 -bind=10.2.118.50 node-id=$(uuidgen | awk '{print tolower($0)}')

如果这两种方法都不好用，应该是因为这个node-id已经存在于数据存储目录下，名字叫node-id, 使用vi或者vim打开可以看到一个guid格式的信息，这里保存的就是node ID信息，所以可以直接删除，然后启动命令。

Note：如果构建Consul集群的话，必须把所有的agent（server）启动之后，访问ui页面（http://localhost:8500）才会work。

      
Consul是HashiCorp公司推出的开源工具，用于实现分布式系统的服务发现与配置。与其他服务分布式注册与发现的方案，比如Airbnb的SmartStack等相比，Consul的方案更一站式，内置了服务注册与发现框架，分布一致性协议实现，健康检查，Key/Value存储，多数据中心方案，不再需要依赖其他工具（比如ZooKeeper等），使用起来比较简单。

   Consul用GoLang实现，因此具有天然可移植性（支持Linux，windows和Mac OS X）；安装包仅包含一个可执行文件，方便部署，与Docker等轻量级容器可无缝配合。

     Ocelot是一个用.NET Core实现并且开源的API网关，它功能强大，包括了：路由，负载均衡，请求聚合，认证，鉴权，限流熔断等，这些功能都只需要简单的配置即可完成。
     Consul是一个分布式，高可用，支持多数据中心的服务注册，发现，健康健康和配置共享的服务软件，由HashiCorp公司用Go语言开发。
    Ocelot集成对Consul支持，在Ocelot gateway中ocelot.json配置可以开启ocelot+consul的组合使用，实现服务注册，服务发现，健康检查，负载均衡。

Consul 端口的使用汇总：

TCP/8300	8300端口用于服务器节点。客户端通过该端口RPC协议调用服务端节点。
TCP/UDP/8301	8301端口用于单个数据中心所有节点之间的互相通信，即对LAN池信息的同步。它使得整个数据中心能够自动发现服务器地址，分布式检测节点故障，事件广播（如leader选择事件）
TCP/UDP/8302	8302端口用于单个或多个数据中心之间的服务器节点的信息同步，即对WAN池信息的同步。它针对互联网的高延迟进行了优化，能够实现跨数据中心请求。
8500	8500端口基于HTTP协议，用于API接口或WEB UI访问。
8600	8600端口作为DNS服务器，它使得我们可以通过节点名查询节点信息。

Consul 内部原理解析：


1、服务器server1,server2, server3上分别部署了consul server，组成consul集群，通过raft选举算法，server2成为leader节点。
2、服务器server4和server5上通过Consul Client分别注册server A，server B， server C（服务A，B，C注册到Consul可以通过HTTP API（8500端口）的方式，也可以通过配置文件的方式）。
3、Consul Client将注册信息通过RPC转发到Consul Server，服务信息保存在server的各个节点中，并且通过Raft实现强一致性。
4、服务器Server 6中Program D要访问Service B，此时Program D先访问本机Consul Client提供的HTTP API，Consul Client会将请求转发到Consul Server。Consul Server查询到Service B并返回，最终Program D拿到了ServiceB的所有部署IP和端口，根据负载均衡策略，选择ServiceB的其中一个并向其发起请求。

  如果服务发现采用的是DNS方式，则Program D中使用ServiceB的服务发现域名，域名解析请求首先先到达本机DNS代理，然后转发到本机Consul Client，Consul Client会将请求转发到consul server。随后的流程和上述一直。
