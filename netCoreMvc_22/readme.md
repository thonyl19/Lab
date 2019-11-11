## [ASP.NET Core 2.2 - MVC](https://docs.microsoft.com/zh-tw/aspnet/core/tutorials/first-mvc-app/?view=aspnetcore-2.2)
> - 本專案是 參照 官方教學課程中 MVC 的章節,逐步開始實作.
> - 因 MVC 排在第二項,故實作參考代碼 以 B 做為識別
> - 此處只摘錄章節中重點備忘,詳細說明請參見官網.

### [B01-建立專案](https://docs.microsoft.com/zh-tw/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-2.2&tabs=visual-studio)
```
	dotnet new mvc
```
 	
### [B02-新增控制器](https://docs.microsoft.com/zh-tw/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-2.2&tabs=visual-studio)
> - 瞭解 controller 和 參數傳遞寫法
> - Mvc 的 View 是可以動態修改,不需重編譯.
> - 多載的方法 無法正常 work, 會出現如下錯誤訊息,相關說明註記於程式以茲備忘.
>> AmbiguousMatchException: The request matched multiple endpoints. Matches:

### [B03-新增檢視](https://docs.microsoft.com/zh-tw/aspnet/core/tutorials/first-mvc-app/adding-view?view=aspnetcore-2.2&tabs=visual-studio)
> - 建立 HelloWorld 頁面
> - @RenderBody 用法
> - ViewData - 將資料從控制器傳遞至檢視
> - 修改主頁面 - Shared/_Layout.cshtml 
> - 建立 Welcome 頁面,和練習  Razor - for 用法 
	
### [B04-新增模型](https://docs.microsoft.com/zh-tw/aspnet/core/tutorials/first-mvc-app/adding-model?view=aspnetcore-2.2&tabs=visual-studio)
> - 以下套件都有版本對應上的問題, 所以必須搭配對應版本,不然執行時會報錯
```
	dotnet tool install --global dotnet-aspnet-codegenerator --version 2.2
	dotnet add package Microsoft.EntityFrameworkCore.SQLite -v 2.2.3
	dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design -v 2.2.0
```
> - 使用相依性插入容器，在 Startup.ConfigureServices 中註冊資料庫內容
> - 建立 DataModel , DBContext  和其他串接設定 
