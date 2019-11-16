## Debug

* HttpListener 的 *:port 监听需要 Administrator 权限，否则会报错误 Access is denied. 
* 解决办法 1. 以 Administrators 启动程序
* 解决办法 2. 以 Administrator 执行以下指令，为当前用户 9988 端口赋权限，然后启动
* 解决办法 3. 设置绑定的 IP 为 localhost

> netsh http add urlacl url=http://*:9988/ user=COMPUTERNAME\useraccount


```
So more than 2 years later, this works for me now on Windows Server 2008 R2 with .NET framework 4.5. 
httpListener.Prefixes.Add("http://*:4444/"); 
indeed shows an Access Denied error but httpListener.Prefixes.Add("http://localhost:4444/"); 
work without any problem. It looks like Microsoft excluded localhost from these restrictions. 
Interestingly, httpListener.Prefixes.Add("http://127.0.0.1:4444/"); 
still shows an Access Denied error, so the only thing that works is localhost:{any port}
```


[相关资料][0]


### bench mark

> ab -c 2000 -n 200000 "http://192.168.39.18:9988/api"


### app.manifest 要求程序以 Administrator 级别启动 (http listener)

``` xml
<requestedExecutionLevel level="requireAdministrator" uiAccess="false" />
```


[0]: https://stackoverflow.com/questions/4019466/httplistener-access-denied

