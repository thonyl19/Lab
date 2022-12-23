set tar_BasePath=H:\GRF_Dev\

set src_path=P:\MyLab\UnitTest\GTI\GTI_Test~.cs
set tar_path=%tar_BasePath%Genesis_MVC\App_Start\GTI_Test~.cs
mklink %tar_path%  %src_path%


set DBC_path=P:\MyLab\UnitTest\GTI\configSetting\Connection~.config
set tar_path=%tar_BasePath%\Genesis_MVC\configSetting\Connection~.config
mklink %tar_path%  %src_path%
 
pause

