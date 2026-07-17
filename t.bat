set tar_BasePath=G:\LIO_Dev
set src_path=G:\LIO_Main\packages\
set tar_path=%tar_BasePath%\packages
mklink /j %tar_path%  %src_path%
 
pause

