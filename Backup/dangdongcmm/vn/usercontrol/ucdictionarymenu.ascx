<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uccataloguemenu.ascx.cs" Inherits="dangdongcmm.uccataloguemenu" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="group">
    <ul>
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
             <li><a href="dictionary-cat-vn-at-<%#Eval("Id")%>.aspx"><%#Eval("Name")%></a> <%#Eval("eIconex") %></li><li class="break">|</li>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <li><a href="dictionary-cat-vn-at-.aspx">Tất cả</a></li>
    </ul>
    
    <script type="text/javascript">
    function listdicategory(did) {
    }
    </script>
</ASP:PANEL>