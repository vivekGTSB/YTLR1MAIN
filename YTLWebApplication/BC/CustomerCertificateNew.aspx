<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.CustomerCertificateNew" Codebehind="CustomerCertificateNew.aspx.vb" %>

<!DOCTYPE html >
<html>
<meta name="viewport" content="width=device-width, initial-scale=1">
<head runat="server">
    <title>Print Certificate</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.2.0/css/datepicker.min.css"
        rel="stylesheet" type="text/css" />
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css"
        rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="//cdn.datatables.net/plug-ins/1.10.7/integration/bootstrap/3/dataTables.bootstrap.css" />
    <script type="text/javascript" language="javascript" src="//code.jquery.com/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="https://netdna.bootstrapcdn.com/bootstrap/3.0.3/js/bootstrap.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.2.0/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" language="javascript" src="//cdn.datatables.net/1.10.7/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" language="javascript" src="//cdn.datatables.net/plug-ins/1.10.7/integration/bootstrap/3/dataTables.bootstrap.js"></script>
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="js/chosennew.js" type="text/javascript"></script>
    
    <style type="text/css">
     @import "css/chosen.min.css";
        .chzn-results
        {
            width: 440px;
        }
        .chzn-container
        {
            width: 450px;
        }
        .chzn-search
        {
            width: 370px;
        }
    </style>
    
    <script type="text/javascript">
        function mysubmit() {
            var userid = document.getElementById("ddlUsers").value;
            var plateno = document.getElementById("ddlPlate").value;
            var companyname = $('#ddlUsers option:selected').text();
            $('#userid').val(userid);
            $('#plateno').val(plateno);
            $('#companyname').val(companyname);
            var pdfformobj = document.getElementById("pdf");
            pdfformobj.submit();
        }
          
        $(function () {
            
            $(".chzn-select").chosen();

        });

        function testAnim() {
            $('#animationSandbox').removeClass().addClass('zoomIn animated').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
                $(this).removeClass();
            });
        };
       

    </script>
    <style>
        .ui-datepicker-calendar
        {
            display: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="form-horizontal">
    <center>
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-12 col-sm-12">
                    <h3 class="info">
                        <p class="bg-primary">
                            Download Certificate
                        </p>
                    </h3>
                </div>
            </div>
            <div class="row">
                <div class="col-md-2">
                </div>
                <div class="col-md-8">
                    <table border="0" cellpadding="5" cellspacing="5" class="table bg-success table-responsive">
                        <tr>
                            <td>
                                <div class="form-group">
                                    <label for="inputEmail3" class="col-sm-2 control-label">
                                        User Name</label>
                                    <div class="col-sm-10">
                                        <asp:DropDownList runat="server" ID="ddlUsers" AutoPostBack="True" CssClass="chzn-select">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                <div class="form-group">
                                    <label for="inputEmail3" class="col-sm-2 control-label">
                                       Plate No</label>
                                    <div class="col-sm-10">
                                        <asp:DropDownList runat="server" ID="ddlPlate" CssClass="chzn-select">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </td>
                        </tr>
                       
                        <tr>
                            <td>
                                <div class="form-group">
                                    <div class="col-sm-offset-2 col-sm-10">
                                        <span id="btnUpload" title="Print" class="btn btn-default"  
                                            onclick="return mysubmit()" >Print</span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                    <asp:Label ID="lblMessage" runat="server" Text="" Font-Names="Arial"></asp:Label>
                </div>
                <div class="col-md-2">
                </div>
            </div>
        </div>
    </center>
    </form>
    <form id="pdf" method="post" action="GussCertificateNew.aspx">
    <input type="hidden" id="userid" name="userid" value="" />
     <input type="hidden" id="plateno" name="plateno" value="" />
    <input type="hidden" id="companyname" name="companyname" value="" />
   
    </form>
</body>
</html>
