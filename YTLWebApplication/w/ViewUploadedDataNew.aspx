<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.ViewUploadedDataNew" Codebehind="ViewUploadedDataNew.aspx.vb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Uploaded Data</title>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" rel="stylesheet" />
     <script src="https://unpkg.com/jspdf@latest/dist/jspdf.min.js"></script>
   
    <script>

        var getImageFromUrl = function (url, callback) {
            var img = new Image();

            img.onError = function () {
                alert('Cannot load image: "' + url + '"');
            };
            img.onload = function () {
                callback(img);
            };
            img.src = url;
        }


        var createPDF = function (imgData) {
            var doc = new jsPDF();
            doc.addImage(imgData, 'JPEG', 10, 10, 50, 50, 'monkey');
            doc.addImage('monkey', 70, 10, 100, 120); // use the cached 'monkey' image, JPEG is optional regardless
            doc.output('datauri');
        }

        
        function convertToPdf() {
            var image = new Image();
            image.src = document.getElementById("imgData").src;
            document.body.appendChild(image);

            var pdf = new jsPDF();
            pdf.addImage(image, 'PNG', 0, 0);
            pdf.save("download.pdf");
        }
</script>
</head>
<body class="container container-fluid">
    <form id="form1" runat="server">
        <div class="badge alert-danger">
            Uploaded Data By Driver
        </div>
        <div class="row">
            <div class="col-md-12">
                <table class="table table-responsive" style="margin-top: -10px !important;">
                    <tr>
                        <td>Driver IC</td>
                        <td>
                            <asp:Label Text="" ID="lblIC" CssClass="text-info" runat="server" />
                        </td>
                         <td>QR Code</td>
                        <td>
                            <asp:Label Text="" ID="lblQR" CssClass="text-info" runat="server" />
                        </td>
                       
                    </tr>   
                    <tr>
                        <td>DN No</td>
                        <td>
                            <asp:Label Text="" ID="lblDnno" CssClass="text-info" runat="server" />
                        </td>
                         <td>Ship to Name</td>
                        <td>
                            <asp:Label Text="" ID="lblShipTo" CssClass="text-info" runat="server" />
                        </td>
                       
                    </tr>   
                    <tr>
                        <td>Diversion</td>
                        <td>
                             <asp:RadioButtonList runat="server" ID="IsDiversion" RepeatDirection="Horizontal">
                                <asp:ListItem Text="Yes"  Value="1"/>
                                <asp:ListItem Text="No" Selected="True" Value="0" />
                            </asp:RadioButtonList>
                        </td>
                       <td> Plateno: <asp:Label Text="" ID="lblPlateno" Font-Bold="true"  CssClass="text-info" runat="server" /></td>
                        <td>
                           Driver Name: <asp:Label Text="" ID="lblDriverName"  Font-Bold="true" CssClass="text-info" runat="server" />
                        </td>
                    </tr>
                    <br />
                    <tr>      
                        
                        <td colspan="3">
                              <asp:Image ImageUrl="" style='height:360px;' ID="imgData" CssClass="img-responsive" runat="server"  />
                              
                        </td>
                        <td colspan="2">
                            <asp:Image ImageUrl="" style='height:360px;' ID="imgBW" CssClass="img-responsive" runat="server"  />
                        </td>
                        
                        
                    </tr>
                </table>
            </div>
             
        </div>
    </form>
</body>
</html>
