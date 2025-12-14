# Unity AppLovin 试玩广告

# 1: 第1种办法： 官方插件：https://playground.lunalabs.io/downloads

(1) 报错: .Net 目标框架 v4.7 缺失的问题？

解决办法：https://dotnet.microsoft.com/zh-cn/download/visual-studio-sdks,   一定要下载安装 4.7 ，而不是 4.7.2类似的，否则问题还会出现。

(2) 报错: 某些API 不存在？

解决办法：手动双击错误，自动开启VS到指定错误代码，全部注销即可。

(3) Build 完成后，上传后，得付费才能导出 Applovin HTML. (直接把路堵死)

# 2: 第2种办法：直接导出 WebGL 遇到的问题

(1) 合并所有资源到Single HTML: https://github.com/Manurocker95/Export-Unity-WebGL-As-Single-HTML

(2) 打完包后，尺寸一直大于5M 不合规。


