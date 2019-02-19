<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>СТАТИСТИКА</title>
</head>
<body style="background: url(/Content/IMG/backpic.png)">
    <form id="form1" runat="server">
    <div>
    
        <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource2">
            <Columns>
                <asp:BoundField DataField="Gameresult" HeaderText="Результат" SortExpression="Gameresult" >
                <ControlStyle BackColor="White" />
                <HeaderStyle BackColor="#CCFFFF" />
                <ItemStyle BackColor="White" />
                </asp:BoundField>
                <asp:BoundField DataField="Column1" HeaderText="Сколько раз" ReadOnly="True" SortExpression="Column1" >
                <HeaderStyle BackColor="#CCFFFF" />
                <ItemStyle BackColor="White" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT Gameresult, Count(Id) FROM Result GROUP BY Gameresult"></asp:SqlDataSource>
        <br />
        <br />
    
    </div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="SqlDataSource1">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Игра" ReadOnly="True" SortExpression="Id" >
                <HeaderStyle BackColor="#CCFFFF" />
                <ItemStyle BackColor="White" />
                </asp:BoundField>
                <asp:BoundField DataField="Gamer" HeaderText="Ходы" SortExpression="Gamer" >
                <HeaderStyle BackColor="#CCFFFF" />
                <ItemStyle BackColor="White" />
                </asp:BoundField>
                <asp:BoundField DataField="Gameresult" HeaderText="Результат игры" SortExpression="Gameresult" >
                <HeaderStyle BackColor="#CCFFFF" />
                <ItemStyle BackColor="White" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT [Gamer], [Gameresult], [Id] FROM [Result] ORDER BY [Id] DESC"></asp:SqlDataSource>
    </form>
</body>
</html>
