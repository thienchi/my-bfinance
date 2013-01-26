<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucsetupmember.ascx.cs" Inherits="dangdongcmm.cmm.ucsetupmember" %>
<%@ REGISTER Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<ASP:PANEL runat="server" CssClass="modalPopup" ID="pnlPopup" style="display:none;width:500px;">
    <ASP:PANEL runat="Server" ID="pnlDragHandle" CssClass="modalDragHandle">
        <b>Settings members's informations</b>
    </ASP:PANEL>
    <div class="tformin" style="margin:10px 0; max-height:100px; overflow:auto;">
    <ASP:REPEATER id="rptView" runat="server" >
        <ITEMTEMPLATE>
            <li>[ <%#Eval("Id") %> ] &nbsp; <a><%#Eval("Name") %></a>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    </div>

    <table cellspacing="0" cellpadding="0" border="0" width="100%" class="tformin">
        <tr>
            <td>
                <ASP:PANEL ID="pnlStatus" runat="server">
                    <fieldset class="fieldset1">
                        <legend><b>Allow / Ban members</b></legend>
                        <div>
                        <ASP:RADIOBUTTONLIST ID="radStatus" runat="server" CssClass="control" RepeatDirection="Horizontal">
                            <ASP:LISTITEM Value="1">Allow</ASP:LISTITEM>
                            <ASP:LISTITEM Value="4">Ban</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST></div>
                    </fieldset>
                </ASP:PANEL>
            </td>
        </tr>
        <tr>
            <td align="right">
                <hr />
                <ASP:BUTTON ID="cmdSave" runat="server" Text="Save setting" CssClass="button" OnClientClick="javascript:return hideModaltoProccessing('modalBehavior')" OnClick="cmdSave_Click" /> 
                <ASP:BUTTON ID="cmdCancel" runat="server" Text=" Close " CssClass="button" OnClientClick="javascript:return hideModal('modalBehavior');" />
            </td>
        </tr>
    </table>
</ASP:PANEL>
<AJAXTOOLKIT:MODALPOPUPEXTENDER runat="server" ID="MODALPOPUPEXTENDER1"
    BehaviorID="modalBehavior"
    TargetControlID="cmdTargetControlForModalPopup"
    PopupControlID="pnlPopup" CacheDynamicResults="true" 
    Y="20"
    RepositionMode="RepositionOnWindowResizeAndScroll" 
    BackgroundCssClass="modalBackground"
    DropShadow="True"
    >
</AJAXTOOLKIT:MODALPOPUPEXTENDER>
<ASP:BUTTON runat="server" ID="cmdTargetControlForModalPopup" Text="Show Modal" style="display:none" />
