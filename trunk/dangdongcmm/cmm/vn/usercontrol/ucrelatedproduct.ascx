<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucrelatedproduct.ascx.cs" Inherits="dangdongcmm.cmm.ucrelatedproduct" %>
<%@ IMPORT Namespace="dangdongcmm.utilities" %>

<ASP:PANEL ID="pnlRelated" runat="server" Visible="true" CssClass="rowhor">
    <div class="title">
    <div class="dformoutcommand">
        <div class="colfilter" style="width:100%">
            <div><ASP:DROPDOWNLIST ID="RP_ddlCid" runat="server" onchange="RPloadlist(this)"></ASP:DROPDOWNLIST></div>
            <div style="margin-top:10px;"><select id="RP_listviewProduct" multiple="multiple" style="height:250px"></select></div>
            <div style="margin-top:10px;"><input type="button" value=" Thêm vào Danh Sách Liên Quan " onclick="RPgetlist()" class="buttoncmd" /></div>
        </div>
    </div>
    </div>
    <div class="boxctrll">
        <div id="RP_error"></div>
        <table class="RP_listselectedProduct">
            <tr class="rprowi">
                <th class="rpbase">Hình ảnh</th><th class="rpnote">Tên sản phẩm</th><th class="rpsort">Danh mục</th><th class="rpcomd">Xóa</th>
            </tr>
        </table>
        <div id="RP_containerProduct" runat="server" title="0">
            <table id="RP_listselectedProduct" class="RP_listselectedProduct"></table>
        </div>
    </div>
    <ASP:HIDDENFIELD ID="RP_txtRelatedid" runat="server" />
    
    <script type="text/javascript">
    function RPloadlist(e) {
        $(e).upload('../RPLoadHandler.ashx?cid=' + $(e).val(), function(res) {
            if(VALIDATA.IsNullOrEmpty(res)) { $('#RP_listviewProduct').children().remove(); return; }
            var json = eval("(" + res.substring(5, res.length - 6) + ")");
            $('#RP_listviewProduct').children().remove();
            for(var i=0; i < json.productinfo.length; i++) {
                $('<option value="' + json.productinfo[i].productid + '">' + json.productinfo[i].productname + '</option>').appendTo('#RP_listviewProduct');
            }
        }, 'html');
    }
    function RPgetlist() {
        var selectedId = RPgetselectedid();
        $('#RP_listviewProduct').upload('../RPLoadHandler.ashx?iid=' + selectedId, function(res) {
            if(VALIDATA.IsNullOrEmpty(res)) return;
            var json = eval("(" + res.substring(5, res.length - 6) + ")");
            for(var i=0; i < json.productinfo.length; i++) {
                var row = $('<tr id="rprowi'+FAindex+'" class="rprowi"></tr>').appendTo('#RP_listselectedProduct');
                var fileurlbase = json.productinfo[i].productimage;
                $('<td class="rpbase"><img id="rpimag'+i+'" src="' + (fileurlbase.indexOf('http://')==0 ? fileurlbase : ('<%=CConstants.WEBSITE %>/' + fileurlbase)) + '" /></td>').appendTo(row);
                $('<td class="rpnote">' + json.productinfo[i].productname + '</td>').appendTo(row);
                $('<td class="rpsort">' + json.productinfo[i].productcategory + '</td>').appendTo(row);
                $('<td class="rpcomd"><a href="javascript:RPremove(' + json.productinfo[i].productid + ',' + FAindex + ');">x</a></td>').appendTo(row);
                FAindex++;
            }
            $('#RP_error').html('Bạn cần nhấn nút "Lưu" để lưu lại danh sách này.');
            RPchanged = true;
        }, 'html');
    }
    function RPgetselectedid() {
        var relatedId = $('#RP_listviewProduct').val().toString();
        var selectedId = $('#<%=RP_txtRelatedid.ClientID %>').val();
        var newselectedId = '';
        if( !VALIDATA.IsNullOrEmpty(selectedId) ) {
            var strrelatedId = $('#<%=RP_txtRelatedid.ClientID %>').val();
            var arrrelatedId = relatedId.split(',');
            for(var i=0; i<arrrelatedId.length; i++) {
                if((',' + strrelatedId + ',').indexOf(',' + arrrelatedId[i] + ',') == -1) {
                    selectedId += ',' + arrrelatedId[i];
                    newselectedId += (newselectedId=='' ? '' : ',') + arrrelatedId[i];
                }    
            }
        }
        else {
            selectedId = relatedId;
            newselectedId = relatedId;
        }
        $('#<%=RP_txtRelatedid.ClientID %>').val(selectedId);
        return newselectedId;
    }
    
    var FAindex = Math.abs($('#<%=RP_containerProduct.ClientID %>').attr('title'));
    var RPchanged = false;
    function RPremove(productid, index) {
        var strrelatedId = (',' + $('#<%=RP_txtRelatedid.ClientID %>').val() + ',').replace(',' + productid + ',', ',');
        $('#<%=RP_txtRelatedid.ClientID %>').val(strrelatedId == ',' ? '' : strrelatedId.substring(1, strrelatedId.length - 1));
        $('#RP_listselectedProduct').find('#rprowi' + index).remove();
        $('#RP_error').html('Bạn cần nhấn nút "Lưu" để hoàn tất việc xóa sản phẩm liên quan.');
        RPchanged = true;
    }
    </script>
</ASP:PANEL>