<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsl.ascx.cs" Inherits="dangdongcmm.ucnewsl" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="relatedl width100">
    <ASP:DATALIST ID="rptList" runat="server" CssClass="ldatalist" SelectedIndex="0" >      
        <ITEMTEMPLATE>
            <div class="article" style="margin-bottom:15px; padding-bottom:15px; border-bottom:1px solid #f2f2f2;">
                <%#Eval("eFilepreview").ToString() == "" ? "" : ("<div class=\"blockimg\"><a href=\"/" + Eval("eUrl2") + "\">" + Eval("eFilepreview") + "</a></div>")%>
                <%--<div class="lname"><a href="/<%#Eval("eUrl2") %>?pag=<%=PageIndex %>"><%#Eval("Name")%></a> <%#Eval("eIconex") %></div>--%>
                <%--<div class="intr"><%#Eval("Introduce") %> <a class="more" href="/<%#Eval("eUrl2") %>?pag=<%=PageIndex %>">...chi tiết</a></div>--%>
                <div class="lname"><a href="/<%#Eval("eUrl2") %>"><%#Eval("Name")%></a> <%#Eval("eIconex") %></div>
                <div class="intr"><%#Eval("Introduce") %> <a class="more" href="/<%#Eval("eUrl2") %>"> ...chi tiết</a></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:DATALIST>
    <div align="right" class="pagercontainer">
        <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="<b>$RECORDCOUNT$</b> bài viết" />
    </div>
</ASP:PANEL>