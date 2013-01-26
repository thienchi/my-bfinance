<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsa.ascx.cs" Inherits="dangdongcmm.ucnewsa" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="relatedl width100">
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="newsa">
                <div class="inlist"><a href="<%#Eval("eUrl") %>"><%#Eval("Name")%></a> <%#Eval("eIconex") %></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" Visible="false" PageSize="6" />
    
    <script type="text/javascript">
    $(document).ready(function() {
        $(".newsa:last").css("border","none");
    });
    </script>
    
</ASP:PANEL>