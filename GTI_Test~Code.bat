set tar_BasePath=N:\CUB_Dev

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

set src_path=P:\MyLab\UnitTest\GTI\configSetting\GTiMESReg.reg
set tar_path=%tar_BasePath%\Genesis_MVC\GTiMESReg.reg
del %tar_path% \Q
mklink %tar_path%  %src_path%

set src_path=P:\MyLab\GTI_Sample\Self\_ViewStart.cshtml
set tar_path=%tar_BasePath%\Genesis_MVC\Areas\Example\Views\_ViewStart.cshtml
del %tar_path% \Q
mklink %tar_path%  %src_path%

set src_path=M:\Prd_Dev\.git\info\exclude
set tar_path=%tar_BasePath%\.git\info\exclude
del %tar_path% \Q
mklink %tar_path%  %src_path%
 
set src_path=P:\MyLab\GTI_Sample\Self
set tar_path=%tar_BasePath%\Genesis_MVC\Areas\Example\Views\Self
rem 如果目錄己存在  需要手動刪除
rem del %tar_path% \Q
mklink /j %tar_path%  %src_path%
 
pause

