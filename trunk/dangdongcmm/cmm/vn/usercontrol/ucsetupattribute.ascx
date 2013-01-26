<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucsetupattribute.ascx.cs" Inherits="dangdongcmm.cmm.ucsetupattribute" %>
<%@ REGISTER Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<ASP:PANEL runat="server" CssClass="modalPopup" ID="pnlPopup" style="display:none;width:500px;">
    <ASP:PANEL runat="Server" ID="pnlDragHandle" CssClass="modalDragHandle">
        <b>Điều chỉnh các thiết lập thông tin</b>
    </ASP:PANEL>
    <div class="tformin" style="margin:10px 0; max-height:100px; overflow:auto;">
    <ASP:REPEATER id="rptView" runat="server">
        <ITEMTEMPLATE>
            <li>[ <%#Eval("Id") %> ] &nbsp; <a><%#Eval("Name") %></a>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    </div>
    
    <div class="tformin">
    <ASP:PANEL ID="pnlSymbol" runat="server" CssClass="row">
        <div class="title"><b>Biểu tượng kèm theo thông tin</b></div>
        <div class="boxctrll"><ASP:RADIOBUTTONLIST ID="radSymbol" runat="server" RepeatDirection="Horizontal" RepeatColumns="5" CssClass="ctrlrad"></ASP:RADIOBUTTONLIST></div>
    <div class="dbreakh"></div>
    </ASP:PANEL>
    <ASP:PANEL ID="pnlMarkas" runat="server" CssClass="row">
        <div class="title"><b>Vị trí hiển thị đặc biệt của thông tin</b></div>
        <div class="boxctrll"><ASP:RADIOBUTTONLIST ID="radMarkas" runat="server" RepeatDirection="Horizontal" CssClass="ctrlrad">
            <ASP:LISTITEM Value="0">Bình thường</ASP:LISTITEM>
            <ASP:LISTITEM Value="1">Lên đầu trang chủ</ASP:LISTITEM>
            <ASP:LISTITEM Value="2">Tiêu điểm, nổi bật</ASP:LISTITEM>
        </ASP:RADIOBUTTONLIST></div>
    <div class="dbreakh"></div>
    </ASP:PANEL>
    <ASP:PANEL ID="pnlStatus" runat="server" CssClass="row">
        <div class="title"><b>Trạng thái hiển thị thông tin</b></div>
        <div class="boxctrll"><ASP:RADIOBUTTONLIST ID="radStatus" runat="server" RepeatDirection="Horizontal" CssClass="ctrlrad">
            <ASP:LISTITEM Value="1">Hiển thị</ASP:LISTITEM>
            <ASP:LISTITEM Value="4">Không hiển thị</ASP:LISTITEM>
        </ASP:RADIOBUTTONLIST></div>
    </ASP:PANEL>
    <div class="dbreakh"></div>
    <ASP:PANEL ID="pnlExtendattr" runat="server" Visible="false" CssClass="row">
        <div class="title"><b>Danh sách mở rộng</b></div>
        <div class="boxctrll"><ASP:CHECKBOXLIST ID="chkExtendattr" runat="server" RepeatDirection="Horizontal" RepeatColumns="4"></ASP:CHECKBOXLIST></div>
    </ASP:PANEL>
    <ASP:PANEL ID="pnlUser" runat="server" Visible="false" CssClass="row">
            <div class="title"><b>Giao quyền quản lý tin cho</b></div>
            <div class="boxctrls"><ASP:DROPDOWNLIST ID="ddlUser" runat="server"></ASP:DROPDOWNLIST></div>
    </ASP:PANEL>
    <div class="rowcmd" align="right">
        <ASP:BUTTON ID="cmdSave" runat="server" Text="Lưu thiết lập" OnClientClick="javascript:return hideModaltoProccessing('modalBehavior')" OnClick="cmdSave_Click" /> 
        <ASP:BUTTON ID="cmdCancel" runat="server" Text=" Đóng " OnClientClick="javascript:return hideModal('modalBehavior');" />
    </div>
    </div>
    <br />&nbsp;
    
</ASP:PANEL>
<AJAXTOOLKIT:MODALPOPUPEXTENDER runat="server" ID="MODALPOPUPEXTENDER1"
    BehaviorID="modalBehavior"
    TargetControlID="cmdTargetControlForModalPopup"
    PopupControlID="pnlPopup" CacheDynamicResults="true" 
    Y="20"
    RepositionMode="RepositionOnWindowResizeAndScroll" 
    BackgroundCssClass="modalBackground"
    DropShadow="True">
</AJAXTOOLKIT:MODALPOPUPEXTENDER>
<ASP:BUTTON runat="server" ID="cmdTargetControlForModalPopup" Text="Show Modal" style="display:none" />
