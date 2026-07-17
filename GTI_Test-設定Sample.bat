set tar_PJ=MPI
set tar_Name=_Mutlselect
set tar_BasePath=P:\MyLab\GTI_Sample\Self
rem set tar_BasePath=P:\MyLab\GTI_Sample\Self\InOut\%tar_PJ%
set tar_MainPath=R:\%tar_PJ%_Dev
 
rem R:\MPI_Dev\Genesis_MVC\Views\Shared\part\_Mutlselect.cshtml
set src_path=%tar_MainPath%\Genesis_MVC\Views\Shared\part\%tar_Name%.cshtml
set tar_path=%tar_BasePath%\%tar_Name%_src.cshtml
del %tar_path% \Q
mklink %tar_path%  %src_path%
 
pause

