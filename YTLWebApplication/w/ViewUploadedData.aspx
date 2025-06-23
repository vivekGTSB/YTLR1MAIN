<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.ViewUploadedData" Codebehind="ViewUploadedData.aspx.vb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Uploaded Data</title>
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="container container-fluid">
    <form id="form1" runat="server">
        <div class="badge alert-danger">
            Uploaded Data By Driver
        </div>
        <div class="row">
            <div class="col-md-12">
                <table class="table table-responsive">
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
                    <br />
                    <tr>
                        <td>Image</td>
                        <td colspan="5">
                              <asp:Image ImageUrl="" style='height:380px;' ID="imgData" CssClass="img-responsive" runat="server"  />
                        </td>
                    </tr>
                </table>
            </div>
             
        </div>
    </form>
</body>
</html>
