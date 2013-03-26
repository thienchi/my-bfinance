<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uccataloguemenu.ascx.cs" Inherits="dangdongcmm.uccataloguemenu" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="group">
    <ul>
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
             <li>              
             <a href="/<%#Eval("eUrlDictionary")%>"><%#Eval("Name")%></a> (<%#Eval("dictCountStr")%>)
             <%#Eval("eIconex") %>
             </li><li class="break">|</li>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <li><a href="/tu-dien-thuat-ngu-vn/Tat-ca.aspx">Tất cả</a> (<%=CountDic %>)</li> 
    </ul>
    
    <script type="text/javascript">
    function listdicategory(did) {
    }
    </script>
</ASP:PANEL>