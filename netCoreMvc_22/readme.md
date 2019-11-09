## [ASP.NET Core 2.2 - MVC](https://docs.microsoft.com/zh-tw/aspnet/core/tutorials/first-mvc-app/?view=aspnetcore-2.2)
> - 本專案是 參照 官方教學課程中 MVC 的章節,逐步開始實作.
> - 因 MVC 排在第二項,故實作參考代碼 以 B 做為識別

### [B01-建立專案](https://docs.microsoft.com/zh-tw/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-2.2&tabs=visual-studio)
```
	dotnet new mvc
```
> 
	
### [B02-新增控制器](https://docs.microsoft.com/zh-tw/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-2.2&tabs=visual-studio)
> - 瞭解 controller 和 參數傳遞寫法
> - Mvc 的 View 是可以動態修改,不需重編譯.
> - 多載的方法 無法正常 work, 會出現如下錯誤訊息,相關說明註記於程式以茲備忘.
>> AmbiguousMatchException: The request matched multiple endpoints. Matches:


	
