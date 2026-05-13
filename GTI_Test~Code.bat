set tar_PJ=MPI
set tar_BasePath=R:\%tar_PJ%_Dev
set tar_MainPath=R:\%tar_PJ%_Main


set src_BasePath=P:\MyLab\UnitTest\GTI\configSetting
set src_DevPath=M:\Prd_Dev

set src_path=%src_BasePath%\GTI_Test.cs
set tar_path=%tar_BasePath%\Genesis_MVC\App_Start\GTI_Test~.cs
del %tar_path% \Q
mklink %tar_path% %src_path%  

set src_path=%src_DevPath%\Genesis_MVC\Scripts\vue_mes\GT_FormCol.js
set tar_path=%tar_BasePath%\Genesis_MVC\Scripts\vue_mes\GT_FormCol~
del %tar_path% \Q
mklink %tar_path% %src_path%  

set src_path=%src_BasePath%\GTI_Test~%tar_PJ%.cs
IF NOT EXIST %src_path% (
    echo %src_path% 不存在，改設為 Prd
	copy %src_BasePath%\GTI_Test~Prd.cs %src_path%
    set src_path=%src_BasePath%\GTI_Test~Prd.cs
) 

set tar_path=%tar_BasePath%\Genesis_MVC\App_Start\GTI_Test~PJ.cs
del %tar_path% \Q
mklink %tar_path% %src_path%  

rem set src_path=%src_BasePath%\BundleConfig.cs
rem set tar_path=%tar_BasePath%\Genesis_MVC\App_Start\BundleConfig~.cs
rem del %tar_path% \Q
rem mklink %tar_path% %src_path% 

set src_path=%src_BasePath%\Connection~.config
set tar_path=%tar_BasePath%\Genesis_MVC\configSetting\Connection~.config
del %tar_path% \Q
mklink %tar_path%  %src_path%

set src_path=%src_BasePath%\GTiMESReg.reg
set tar_path=%tar_BasePath%\Genesis_MVC\GTiMESReg.reg
del %tar_path% \Q
mklink %tar_path%  %src_path%

set src_path=%src_BasePath%\GTiMESReg.reg
set tar_path=%tar_MainPath%\Genesis_MVC\GTiMESReg~.reg
del %tar_path% \Q
mklink %tar_path%  %src_path%

set src_path=P:\MyLab\GTI_Sample\Self\_ViewStart.cshtml
set tar_path=%tar_BasePath%\Genesis_MVC\Areas\Example\Views\_ViewStart.cshtml
del %tar_path% \Q
mklink %tar_path%  %src_path%



set src_path=%src_BasePath%\GTI_exclude
set tar_path=%tar_BasePath%\.git\info\exclude
del %tar_path% \Q
mklink %tar_path%  %src_path%
 
set src_path=P:\MyLab\GTI_Sample\Self
set tar_path=%tar_BasePath%\Genesis_MVC\Areas\Example\Views\Self
rem 如果目錄己存在  需要手動刪除
rem del %tar_path% \Q
mklink /j %tar_path%  %src_path%
 
pause

