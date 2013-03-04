<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsh.ascx.cs" Inherits="dangdongcmm.ucnewsh" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="relatedl width100">
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <%#Container.ItemIndex == 0 ? "<div class=\"newsselhome\" style=\"margin-bottom:20px;\">" 
                + (Eval("eFilepreview").ToString() == "" ? "" : ("<div class=\"blockimg\"><a href=\"/"
                            + Eval("eUrl2") + "\">" + Eval("eFilepreview") + "</a></div>")) 
                                        + "<div class=\"lname\"><a href=\"/" + Eval("eUrl2") 
                                                    + "\">" + Eval("Name") + "</a> " + Eval("eIconex")
                                                                + "</div><div class=\"intr\">" + Eval("Introduce") 
                                                                            + "</div></div>" : "<div class=\"newsselhome\"><div class=\"inlist\"><a href=\"/" 
                                                                                        + Eval("eUrl2") + "\">" + Eval("Name") + "</a> " 
                                                                                                    + Eval("eIconex") + "</div></div>"%>
        </ITEMTEMPLATE>
    </ASP:REPEATER>

    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" PageSize="5" Visible="false" />

</ASP:PANEL>
<script type="text/javascript">
$(document).ready(function() {
    $(".newsfb:last").css("border","none");
});
</script>
