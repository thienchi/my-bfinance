<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucpager.ascx.cs" Inherits="dangdongcmm.cmm.ucpager" %>

<ASP:PANEL ID="pnlList" runat="server" >

    <table cellspacing="0" cellpadding="5" border="0">
        <tr>
            <td><ASP:LABEL ID="lblInfotemplate" runat="server"></ASP:LABEL></td>
            <td>
                <ASP:LINKBUTTON ID="cmdPrev" runat="server" Text="Prev" OnClick="cmdPrev_Click"></ASP:LINKBUTTON>
                <ASP:LINKBUTTON ID="cmdFirst" runat="server" Text="<<" OnClick="cmdFirst_Click"></ASP:LINKBUTTON>
            </td>
            <td>
                [
                <ASP:REPEATER ID="rptList" runat="server" OnItemCommand="rptList_ItemCommand">
                    <ITEMTEMPLATE>
                        <ASP:LINKBUTTON ID="link" runat="server" CommandArgument='<%#Eval("Value") %>' CommandName="Pageindex" Text='<%#Eval("Text") %>' Enabled='<%#Convert.ToInt32(Eval("Value")) == PageIndex ? false : true %>'></ASP:LINKBUTTON>
                    </ITEMTEMPLATE>
                </ASP:REPEATER>
                ]
            </td>
            <td>
                <ASP:LINKBUTTON ID="cmdLast" runat="server" Text=">>" OnClick="cmdLast_Click"></ASP:LINKBUTTON>
                <ASP:LINKBUTTON ID="cmdNext" runat="server" Text="Next" OnClick="cmdNext_Click"></ASP:LINKBUTTON>
            </td>
        </tr>
    </table>

</ASP:PANEL>