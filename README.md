SIPAA_Installer
===============

SIPAA系统的安装程序。

sipaacfg.db和sipaarecs.db必须拷贝到FreeSWITCH安装目录下的db子目录下。
logfile.conf.xml必须拷贝到FreeSWITCH安装目录下的conf/autoload_configs子目录下。
public.xml必须拷贝到FreeSWITCH安装目录下的conf/dialplan子目录下。
sipaa.lua和tsimplify.lua必须拷贝到FreeSWITCH安装目录下的scripts子目录下。

1、如果注册表项不存在，说明FreeSWITCH没安装。本安装程序不能继续执行下去。
2、安装程序必须检查当前用户是否具有对FreeSWITCH安装目录的写权限。如果不具备写权限，本安装程序不能继续执行下去。
任何一个文件拷贝失败，系统必须回滚。也就是说，每次拷贝前请将目标目录内的待覆盖文件拷贝到本安装程序所在目录。
一旦安装过程中出现异常，备份文件需恢复。所有文件都拷贝成功，安装才算成功。
3、安装前，必须检查FreeSWITCH服务是否仍然在运行。如果在运行给个提示框给用户，安装程序必须停止服务后才能继续执行下去。
4、如果服务已安装，但上述四个目录任意一个不存在，说明这是刚安装完毕的系统。请在后台先启动FreeSWITCH，待启动成功后，
再关闭FreeSWITCH服务。这样一个过程，这些目录就会自动生成。安装过程可以继续下去。
