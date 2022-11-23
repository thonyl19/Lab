## (SSE) Server-Sent Events 實現即時廣播
----
### 結論感想
    是一個可以簡便上手的技術應用,但問題是,有連線數限制的 issue ,
    以至於實務應用上, 就存在先天性的瓶頸, 故先止步於此
----
### 使用說明
- `SSE_Msg.cshtml`

    主要是參考 [Server Sent Events In ASP.NET MVC](https://www.c-sharpcorner.com/blogs/server-side-events-in-asp-net-mvc)
其目的用以瞭解 ,如何在 .net MVC 環境中,實現 SSE 的功能,
需要搭配 在 Controllers 程序中,加上以下程序
```
[AllowAnonymous]
public void SSE_Message()
{
    Response.ContentType = "text/event-stream";

    DateTime startDate = DateTime.Now;
    while (startDate.AddMinutes(1) > DateTime.Now)
    {
        Response.Write(string.Format("data: {0}\n\n", DateTime.Now.ToString()));
        Response.Flush();

        System.Threading.Thread.Sleep(5000);
    }

    Response.Close();
}
```

- `SSE_Main , SSE_Item , SSE_page`

    主要是參考  [在 ASP.NET 用 Server-Sent Events 實現即時廣播](https://blog.darkthread.net/blog/server-sent-events-aspx/)
主要是利用 aspx 的技術基底, 所以, 如果要在 .net MVC 上運行, 
必須先把上述的檔案 ,搬到 \views 以外的地方(原因請參見黑大說明)



