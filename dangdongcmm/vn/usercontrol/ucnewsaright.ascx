<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsa.ascx.cs" Inherits="dangdongcmm.ucnewsa" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server">
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="newsadesc" id="newsadesc<%#Eval("Id")%>">
                <%#Eval("eFilepreview").ToString() == "" ? "" : ("<div class=\"blockimg\"><a href=\"/" + Eval("eUrl2") + "\">" + Eval("eFilepreview") + "</a></div>")%>
                <div class="lname"><a href="/<%#Eval("eUrl2") %>"><%#Eval("Name")%></a> <%#Eval("eIconex")%></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" Visible="false" PageSize="3" />
    <script type="text/javascript">
    $(document).ready(function() {
        $(".newsadesc:last").css("border","none");
    });
    </script>    
</ASP:PANEL>