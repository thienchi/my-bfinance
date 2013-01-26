<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="managerupload.aspx.cs" Inherits="dangdongcmm.cmm.managerupload" %>
<%@ MasterType virtualpath="MasterDefault.master" %>
<%@ REGISTER Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" src="../cssscript/swfobject.js"></script>
<script type="text/javascript">
var pre = 'ctl00_ContentPlaceHolder1_';
var preJQ = "#ctl00_ContentPlaceHolder1_";


function onLoad() {
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
    RebindImgPreview();
    RebindFlashPlayerMU();
}
function EndRequestHandler(sender, args) {
    RebindImgPreview();
    RebindFlashPlayerMU();
    
    var currentpath = $(preJQ + "lblSearchLocation").html();
    if(currentpath=="commup/") {
        $(preJQ + "cmdDeleteFolder").hide();
        $("#cmdRenameFolder").hide();
    }    
    else {
        $(preJQ + "cmdDeleteFolder").show();
        $("#cmdRenameFolder").show();
    }
}
function RebindImgPreview() {
    var des = $("img[id^='imgpreview']");
    des.each(function (di) {
        var imgid = des.eq(di).attr('id');
        var img = new Image();
        img = document.images[imgid];
        var labelid = imgid.replace('imgpreview','');
        $('#' + labelid).html(img.width + 'x' + img.height);

        img.width = img.width>=150 ? 150 : img.width;
    });
}

if (window.addEventListener)
    window.addEventListener("load", onLoad, false)
else if (window.attachEvent)
    window.attachEvent("onload", onLoad)
else if (document.getElementById)
    window.onload=onLoad

function copyToClipboard(sources) {
	if(window.clipboardData && clipboardData.setData) {
		clipboardData.setData("Text", sources);
	}
	else {
		user_pref("signed.applets.codebase_principal_support", true);
		netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');
		var clip = Components.classes['@mozilla.org/widget/clipboard;[[[[1]]]]'].createInstance(Components.interfaces.nsIClipboard);
		if (!clip) return;

		var trans = Components.classes['@mozilla.org/widget/transferable;[[[[1]]]]'].createInstance(Components.interfaces.nsITransferable);
		if (!trans) return;
		trans.addDataFlavor('text/unicode');

		var str = new Object();
		var len = new Object();
		var str = Components.classes["@mozilla.org/supports-string;[[[[1]]]]"].createInstance(Components.interfaces.nsISupportsString);
		var copytext = meintext;
		str.data = copytext;
		trans.setTransferData("text/unicode", str, copytext.length*[[[[2]]]]);

		var clipid = Components.interfaces.nsIClipboard;
		if (!clip) return false;
		clip.setData(trans, null, clipid.kGlobalClipboard);
	}
}

function replaceSources(link, path, name) {
    $("#sourceLink").text(link);
    $(preJQ + "sourcePath").val(path);
    $(preJQ + "sourceName").val(name);
    $find("modalBehavior").show();
}
function addnewSources() {
    var path = $(preJQ + "lblSearchLocation").html();
    $("#sourceLink").text("<%=WEBSITE %>/" + path);
    $(preJQ + "sourcePath").val("../../" + path);
    $(preJQ + "sourceName").val("");
    $find("modalBehavior").show();
}
function addnewFolder() {
    var path = $(preJQ + "lblSearchLocation").html();
    $("#sourceLinkFolder").text("<%=WEBSITE %>/" + path);
    $("#popupFolderTitle").text("Thêm thư mục mới trong");
    $("#popupFolderCurrent").text(path);
    $(preJQ + "sourcePath").val("");
    $find("modalBehaviorFolder").show();
}
function renameFolder() {
    var path = $(preJQ + "lblSearchLocation").html();
    $("#sourceLinkFolder").text("<%=WEBSITE %>/" + path);
    $("#popupFolderTitle").text("Đổi tên thư mục ");
    $("#popupFolderCurrent").text(path);
    $(preJQ + "sourcePath").val(path);
    $find("modalBehaviorFolder").show();
}
function doDeleteFile(){
    return confirm("Bạn có chắc chắn muốn xóa tập tin này?");
}
function doDeleteFolder(){
    return confirm("Bạn có chắc chắn muốn xóa thư mục " + "<%=WEBSITE %>/" + $(preJQ + "lblSearchLocation").html() + " cùng các thư mục con và các tập tin trong đó?");
}
function MB_Download(des) {
    window.open("download.aspx?elink=" + des);
}
</script>

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        
        <div class="relative" style="margin-bottom:10px; width:100%;">
            <b><%=WEBSITE %>/<ASP:LABEL ID="lblSearchLocation" runat="server" CssClass="searchbutton"></ASP:LABEL></b>
            ::::::
            | <a id="cmdRenameFolder" href="javascript:void(0);" onclick="javascript:renameFolder();" style="display:none"><b>Rename This Folder</b></a> 
            | <ASP:LINKBUTTON ID="cmdDeleteFolder" runat="server" Text="<b>Delete This Folder</b>" Style="display:none" OnClientClick="javascript:return doDeleteFolder();" OnClick="cmdDeleteFolder_Click"></ASP:LINKBUTTON>
            | <a href="javascript:void(0);" onclick="javascript:addnewFolder();"><b>Add Folder</b></a> 
            | <a href="javascript:void(0);" onclick="javascript:addnewSources();"><b>Add Files</b></a> 
        </div>
        
        <div class="relative">
        <table cellspacing="0" cellpadding="0" border="0" width="100%">      
            <tr>
                <td valign="top" width="230px">
                    <ASP:PANEL id="pnlListFolder" runat="server" width="100%">
                        <ASP:DATALIST ID="dtlListFolder" runat="server" CssClass="ldatalist" 
                            OnItemCommand="dtlListFolder_ItemCommand">
                            <ITEMTEMPLATE>
                                <div style="line-height:24px;"><%# Eval("Iconex") %> <ASP:LINKBUTTON ID="LinkButton1" runat="server" Text='<%# Eval("Name") %>' OnClientClick="javascript:return doSubmit();" CommandName="ViewFilesInFolder" CommandArgument='<%# Eval("Path") %>'></ASP:LINKBUTTON></div>
                            </ITEMTEMPLATE>
                        </ASP:DATALIST>
                    </ASP:PANEL>
                </td>
                <td valign="top">
                    <div class="dformoutcommand">
                        <div class="colfilter">
                        <ASP:PANEL ID="panelSearch" runat="server" DefaultButton="cmdSearch">
                            <table cellspacing="2" cellpadding="0" border="0">
                                <tr>
                                    <td><img src="/cmm/images/symsearch.gif" alt="" /></td>
                                    <td><ASP:TEXTBOX ID="txtKeyword" runat="server"></ASP:TEXTBOX></td>
                                    <td><ASP:BUTTON ID="cmdSearch" runat="server" OnClientClick="javascript:return doSubmit();" OnClick="cmdSearch_Click" CssClass="button" Text="Search" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td colspan="2"><ASP:CHECKBOX ID="chkSearchOption" runat="server" CssClass="control" Text="Liệt kê tất cả các tập tin trong thư mục con" /></td>
                                </tr>
                            </table>
                        </ASP:PANEL>
                        </div>
                    </div>    

                    <ASP:PANEL id="pnlListFiles" runat="server" CssClass="dformoutlist" width="100%">
                        <ASP:DATALIST ID="dtlListFiles" runat="server" CssClass="managerupload" Width="100%" RepeatColumns="4" RepeatDirection="Horizontal" 
                            ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Bottom" OnItemCommand="dtlListFiles_ItemCommand">
                            <ITEMTEMPLATE>
                            <div style="margin-top:10px;">
                                <div><a target="_blank" href="<%# Eval("Path") %>"><%# Eval("eFileattach")%></a></div>
                                <div><%# Eval("Name") %> 
                                <br /><label id="<%# Eval("Name").ToString().Replace(" ","_").Replace(".","_") %>"></label>&nbsp;( <%# Math.Round(float.Parse(Eval("Sized").ToString()) / (Eval("Sized").ToString().Length > 6 ? 1024000 : 1024), 2) + (Eval("Sized").ToString().Length > 6 ? " MB" : " KB") %> )</div>
                                <div><a href="javascript:void(0);" onclick="copyToClipboard('<%# WEBSITE + "/" + Eval("Path").ToString().Replace("../","") %>');">Copy link</a> | <a href="javascript:void(0);" onclick="replaceSources('<%# WEBSITE + "/" + Eval("Path").ToString().Replace("../","") %>', '<%# Eval("Path") %>', '<%# Eval("Name") %>')">Replace</a> | <ASP:LINKBUTTON ID="cmdDelete" runat="server" OnClientClick="javascript:return doDeleteFile();" CommandName="DeleteFile" CommandArgument='<%# Eval("Path") %>'>Delete</ASP:LINKBUTTON></div>
                            </div>    
                            </ITEMTEMPLATE>
                        </ASP:DATALIST>
                    </ASP:PANEL>
                </td>
            </tr>
        </table>
        </div>

<ASP:HIDDENFIELD ID="sourcePath" runat="server" />
<ASP:HIDDENFIELD id="sourceName" runat="server" />
<ASP:PANEL runat="server" CssClass="modalPopup" ID="pnlPopup" style="display:none;width:500px;">
    <ASP:PANEL runat="Server" ID="pnlDragHandle" CssClass="modalDragHandle">
        <table cellspacing="0" cellpadding="0" border="0" width="100%">
            <tr>
                <td><b>Cập nhật hình ảnh/clips:</b></td>
                <td width="20px" style="width: 20px; cursor: pointer" onclick="hideModal('modalBehavior');">
                    [<img src="../images/symclose.gif" alt="Close" onclick="hideModal('modalBehavior');" />]
                </td>
            </tr>
        </table>
    </ASP:PANEL>
    <br /><br />
    
    <table cellspacing="0" cellpadding="5" border="0" width="100%" class="tformin">
        <tr>
            <td><label id="sourceLink"></label><br /><br /></td>
            <td></td>
        </tr>
        <tr>
            <td><ASP:FILEUPLOAD ID="fileUpload" runat="server" Style="height:22px" /></td>
            <td style="width:50px;"><ASP:BUTTON ID="cmdUpload" runat="server" Text="Upload..." OnClientClick="javascript:return hideModaltoProccessing('modalBehavior');" OnClick="cmdUpload_Click" /></td>
        </tr>
    </table>
    <br />
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

<ASP:PANEL runat="server" CssClass="modalPopup" ID="pnlPopupFolder" style="display:none;width:400px;">
    <ASP:PANEL runat="Server" ID="pnlDragHandleFolder" CssClass="modalDragHandle">
        <table cellspacing="0" cellpadding="0" border="0" width="100%">
            <tr>
                <td><b><label id="popupFolderTitle"></label>&nbsp;<label id="popupFolderCurrent"></label></b></td>
                <td width="20px" style="width: 20px; cursor: pointer" onclick="hideModal('modalBehaviorFolder');">
                    [<img src="../images/symclose.gif" alt="Close" onclick="hideModal('modalBehaviorFolder');" />]
                </td>
            </tr>
        </table>
    </ASP:PANEL>
    <br /><br />
    
    <table cellspacing="0" cellpadding="5" border="0" width="100%" class="tformin">
        <tr>
            <td><label id="sourceLinkFolder"></label><br /><br /></td>
            <td></td>
        </tr>
        <tr>
            <td><ASP:TEXTBOX ID="txtNewFolder" runat="server"></ASP:TEXTBOX></td>
            <td style="width:50px;"><ASP:BUTTON ID="cmdNewFolder" runat="server" Text=" Đồng ý " OnClientClick="javascript:return hideModaltoProccessing('modalBehaviorFolder');" OnClick="cmdNewFolder_Click" /></td>
        </tr>
    </table>
    <br />
</ASP:PANEL>
<AJAXTOOLKIT:MODALPOPUPEXTENDER runat="server" ID="MODALPOPUPEXTENDER2"
    BehaviorID="modalBehaviorFolder"
    TargetControlID="cmdTargetControlForModalPopupFolder"
    PopupControlID="pnlPopupFolder" CacheDynamicResults="true" 
    Y="20"
    RepositionMode="RepositionOnWindowResizeAndScroll" 
    BackgroundCssClass="modalBackground"
    DropShadow="True"
    >
</AJAXTOOLKIT:MODALPOPUPEXTENDER>
<ASP:BUTTON runat="server" ID="cmdTargetControlForModalPopupFolder" Text="Show Modal" style="display:none" />
        
        </CONTENTTEMPLATE>
        <TRIGGERS>
            <ASP:POSTBACKTRIGGER ControlID="cmdUpload" />
        </TRIGGERS>
    </ASP:UPDATEPANEL>
    
</ASP:CONTENT>
