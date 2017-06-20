<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MAP200.Default" %>

<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MAP-200 Results</title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Default.css" rel="stylesheet" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body>


    <form runat="server">
        <div class="container">
            <div class="row">
                <asp:Label Text="Version 0.4" runat="server" />
                <div class="form-group">
                    <div class="values">
                        <div class="value value--insertion-loss control-label col-md-4">
                            <asp:Label Text="Insertion Loss" runat="server" AssociatedControlID="insertionLossTextBox" />
                            <asp:TextBox ID="insertionLossTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="value value--return-loss control-label col-md-4">
                            <asp:Label Text="Return Loss" runat="server" AssociatedControlID="returnLossTextBox" />
                            <asp:TextBox ID="returnLossTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="value value--length control-label col-md-4">
                            <asp:Label Text="Length" runat="server" AssociatedControlID="lengthTextBox" />
                            <asp:TextBox ID="lengthTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="btn-group btn-center">
                    <asp:Button ID="statusBtn" runat="server" Text="MAP200 Status" CssClass="btn btn-default" OnClick="statusBtn_Click" />
                    <asp:Button ID="pctStatusBtn" runat="server" Text="PCT Status" CssClass="btn btn-info" OnClick="pctStatusBtn_Click" />
                    <asp:Button ID="startPctBtn" runat="server" Text="Start PCT" CssClass="btn btn-primary" OnClick="startPctBtn_Click" />
                    <asp:Button ID="stopPctBtn" runat="server" Text="Stop PCT" CssClass="btn btn-danger" OnClick="stopPctBtn_Click" />
                    <asp:Button ID="runBtn" runat="server" Text="Run Test" CssClass="btn btn-warning" OnClick="runBtn_Click" />
                    <asp:Button ID="clearBtn" runat="server" Text="Clear Fields" CssClass="btn btn-default" OnClick="clearBtn_Click"/>
                </div>
            </div>
            <div class="row row--textBox control-label">
                <asp:Label Text="Log" runat="server" AssociatedControlID="logTextBox" />
                <asp:TextBox ID="logTextBox" runat="server" CssClass="form-control" Rows="20" TextMode="MultiLine" />
            </div>
        </div>
    </form>
    <script src="Scripts/jquery-1.9.1.js"></script>
    <script src="Scripts/Default.js"></script>
</body>

</html>
