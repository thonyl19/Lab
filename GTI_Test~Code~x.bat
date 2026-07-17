set tar_BasePath=K:\MWF_Dev
set src_path=P:\MyLab\UnitTest\GTI\configSetting\BundleConfig.CUB.cs
set tar_path=%tar_BasePath%\Genesis_MVC\App_Start\BundleConfig~CUB.cs
del %tar_path% \Q
mklink %tar_path% %src_path% 
pause

