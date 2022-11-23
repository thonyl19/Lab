set src_path=P:\MyLab\GTI_Sample\GTI_Test~Code.cs
set tar_path=H:\GRF_Dev\Genesis_MVC\App_Start\GTI_Test~Code.cs
echo %path%

mklink %tar_path%  %src_path%
 
pause

