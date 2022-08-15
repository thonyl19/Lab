rem set src_path=H:\新增資料夾\GTI_Test~Code.cs
rem set tar_path=H:\HM_Dev\Genesis_MVC\App_Start\GTI_Test~Code.cs
rem echo %path%

rem mklink /h %tar_path%  %src_path%

set src_path=P:\MyLab\GTI_Sample\Self
set tar_path=H:\HM_Dev\Genesis_MVC\Areas\Example\Views\Self
mklink /j %tar_path%  %src_path%
pause

