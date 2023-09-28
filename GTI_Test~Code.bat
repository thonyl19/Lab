set tar_BasePath=D:\GRF_Dev

set src_path=P:\MyLab\UnitTest\GTI\configSetting\GTI_Test.cs
set tar_path=%tar_BasePath%\Genesis_MVC\App_Start\GTI_Test~.cs
del %tar_path% \Q
mklink %tar_path% %src_path%  

set src_path=P:\MyLab\UnitTest\GTI\configSetting\BundleConfig.cs
set tar_path=%tar_BasePath%\Genesis_MVC\App_Start\BundleConfig~.cs
del %tar_path% \Q
mklink %tar_path% %src_path% 

set src_path=P:\MyLab\UnitTest\GTI\configSetting\Connection~.config
set tar_path=%tar_BasePath%\Genesis_MVC\configSetting\Connection~.config
del %tar_path% \Q
mklink %tar_path%  %src_path%
 
set src_path=P:\MyLab\GTI_Sample\Self
set tar_path=%tar_BasePath%\Genesis_MVC\Areas\Example\Views\Self
del %tar_path% \Q
mklink /j %tar_path%  %src_path%
 
pause

