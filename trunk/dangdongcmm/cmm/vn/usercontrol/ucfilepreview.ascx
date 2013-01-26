<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucfilepreview.ascx.cs" Inherits="dangdongcmm.cmm.ucfilepreview" %>
<%@ IMPORT Namespace="dangdongcmm.utilities" %>

<div class="rowhor">
    <div class="title">
        <div>Hình xem trước:</div>
        <div class="fppreview"><ASP:IMAGE ID="FP_filePreview" runat="server" ImageUrl="~/commup/no_image.gif" AlternateText="" /></div>
    </div>
    <div class="boxctrll">
        <div class="FPfilepreview">
            <div class="fpsource">
                <ASP:RADIOBUTTONLIST ID="FP_radSelectresource" runat="server" RepeatDirection="Horizontal" CssClass="ctrlrad" onchange="FPSelectsource()">
                    <ASP:LISTITEM Value="0" Selected="True">Tải lên</ASP:LISTITEM>
                    <ASP:LISTITEM Value="1">Nhập đường dẫn</ASP:LISTITEM>
                </ASP:RADIOBUTTONLIST>
            </div>
            <ASP:HIDDENFIELD ID="FP_txtFileurl" runat="server" />
            <ASP:HIDDENFIELD ID="FP_txtUp" runat="server" />
            <table class="fpsource">
                <tr>
                    <td class="fpbase">
                        <div id="FP_pnlUpload"><input type="file" id="FP_fileUpload" name="FP_fileUpload" /></div>
                        <div id="FP_pnlGet"><input type="text" id="FP_fileGet" value="http://" /></div>
                    </td>
                    <td class="fpcomd"><input type="button" id="cmdUpload" value="Nhận" class="buttoncmd" onclick="FPgetfile()" /></td>
                </tr>
            </table>
                
            <div id="FP_error"></div>
            <div class="fplink">
                <ASP:HYPERLINK ID="FP_fileLink" runat="server" Target="_blank"></ASP:HYPERLINK> &nbsp; &nbsp; &nbsp; <ASP:HYPERLINK ID="FP_fileRemove" runat="server" NavigateUrl="javascript:FPremove();"></ASP:HYPERLINK>
            </div>
            
        </div>

        <script type="text/javascript" src="../cssscript/jquery.upload-1.0.2.js"></script>
        <script type="text/javascript">
        var FPsource = 0;
        function FPSelectsource() {
            FPsource = $('.FPfilepreview').find('input[type=radio][name$="FP_radSelectresource"]:checked').val();
            if(FPsource==0) { $('#FP_pnlUpload').show(); $('#FP_pnlGet').hide(); }    
            else if(FPsource==1) { $('#FP_pnlUpload').hide(); $('#FP_pnlGet').show(); }    
        }
        function FPgetfile() {
            if(FPsource==0) { FPupload(); } else if(FPsource==1) { FPgeturl(); }
        }
        function FPupload() {
            $('#FP_fileUpload').upload('../FPUploadHandler.ashx?up=' + $('#<%=FP_txtUp.ClientID %>').val(), function(res) {
                var json = eval("(" + res.substring(5, res.length - 6) + ")");
                $('#<%=FP_txtFileurl.ClientID %>').val(json["name"]);
                $('#<%=FP_fileLink.ClientID %>').text('<%=CConstants.WEBSITE %>/' + json["name"]).attr('href', '<%=CConstants.WEBSITE %>/' + json["name"]);
                $('#<%=FP_filePreview.ClientID %>').attr('src', '<%=CConstants.WEBSITE %>/' + json["name"]);
                $('#<%=FP_fileRemove.ClientID %>').text('x');
                $('#FP_error').html(json["error"]);
            }, 'html');
        }
        function FPgeturl() {
            var fileurl = $('#FP_fileGet').val();
            $('#<%=FP_txtFileurl.ClientID %>').val( fileurl );
            $('#<%=FP_fileLink.ClientID %>').text( fileurl ).attr( fileurl );
            $('#<%=FP_filePreview.ClientID %>').attr('src', fileurl);
            $('#<%=FP_fileRemove.ClientID %>').text('x');
            $('#FP_error').html('');
        }
        function FPremove() {
            var src = $('#<%=FP_txtFileurl.ClientID %>').val();
            var pid = $('#ctl00_ContentPlaceHolder1_txtId').val();
            $('#<%=FP_txtFileurl.ClientID %>').upload('../FPRemoveHandler.ashx?fileurl=' + src + '&pid=' + pid, function(res) {
                $('#<%=FP_txtFileurl.ClientID %>').val('');
                $('#<%=FP_fileLink.ClientID %>').text('').attr('href', '');
                $('#<%=FP_filePreview.ClientID %>').attr('src', '<%=CConstants.WEBSITE %>/commup/no_image.gif');
                $('#<%=FP_fileRemove.ClientID %>').text('');
                if(pid==0) $('#FP_error').html('File removed.');
                else $('#FP_error').html('You need to click Save button to complete removing the file.');
            }, 'html');
        }
        </script>
    
    </div>
</div>
