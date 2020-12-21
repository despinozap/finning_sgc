<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RegistrarAccionCorrectiva.aspx.cs" Inherits="NCCSAN.PlanesAccion.RegistrarAccionCorrectiva" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .Popup_background
        {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }
        
        .Popup_frontground
        {
            background-color: White;
            padding: 30px; /*border-style: dotted;
            border-width: 2px;*/
            box-shadow: 10px 10px 5px #555555;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            setDatepickerStyle();
        });


        function setDatepickerStyle() {

        }

        function setDatepickerStyle() {

            $("#<%= txtFechaRealizado.ClientID %>").datepicker
                (
                    {
                        // Formato de la fecha
                        dateFormat: 'dd-mm-yy',
                        // Primer dia de la semana El lunes
                        firstDay: 1,
                        // Dias Largo en castellano
                        dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
                        // Dias cortos en castellano
                        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                        // Nombres largos de los meses en castellano
                        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                        // Nombres de los meses en formato corto 
                        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dec"],
                        firstDay: 1
                    }
                );


            loadDatepickerStyle();

            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

            function EndRequestHandler(sender, args) {
                $("#<%= txtFechaRealizado.ClientID %>").datepicker
                (
                    {
                        // Formato de la fecha
                        dateFormat: 'dd-mm-yy',
                        // Primer dia de la semana El lunes
                        firstDay: 1,
                        // Dias Largo en castellano
                        dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
                        // Dias cortos en castellano
                        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                        // Nombres largos de los meses en castellano
                        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                        // Nombres de los meses en formato corto 
                        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dec"],
                        firstDay: 1
                    }
                );

                loadDatepickerStyle();
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Registro Acción Correctiva</h1>
    </div>
    <asp:HiddenField ID="hfCodigoAccionCorrectiva" runat="server" />
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnAccionCorrectiva" runat="server">
                <table align="center" class="tableControls">
                    <tr>
                        <td colspan="2" class="tdTitle">
                            <h1>
                                Acción Correctiva</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label22" runat="server" Text="Descripción" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="lbDescripcion" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label4" runat="server" Text="Responsable" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="lbNombreResponsable" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label2" runat="server" Text="Fecha límite" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="lbFechaLimite" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label23" runat="server" Text="Fecha de ejecución" 
                                Font-Bold="True"></asp:Label>
                            &nbsp;<asp:Label ID="Label3" runat="server" Text="*" ForeColor="Red" Font-Bold="True" ></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtFechaRealizado" runat="server" Width="190px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label1" runat="server" Text="Observación" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtObservacion" runat="server" Width="300px" Height="113px" MaxLength="3000"
                                TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <!-- Panel Message -->
            <asp:HiddenField ID="hfMessage" runat="server" />
            <asp:ModalPopupExtender ID="hfMessage_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                DynamicServicePath="" Enabled="True" PopupControlID="upMessage" TargetControlID="hfMessage">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upMessage" runat="server" UpdateMode="Always" style="width: 100%">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnMessage" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center">
                        <asp:Label ID="lbMessage" runat="server" CssClass="messageBoxTitle"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btMessageAceptar" runat="server" Text="Aceptar" Width="70px" CssClass="submitButton"
                                        OnClick="btMessageAceptar_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <br />
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Message !-->
            <!-- Panel Confirmar Volver -->
            <asp:HiddenField ID="hfConfirmVolver" runat="server" />
            <asp:ModalPopupExtender ID="hfConfirmVolver_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                DynamicServicePath="" Enabled="True" TargetControlID="hfConfirmVolver" PopupControlID="pnConfirmVolver">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upConfirmVolver" runat="server" style="width: 100%" UpdateMode="Conditional">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnConfirmVolver" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center"
                        Width="100%">
                        <asp:Label ID="lbConfirmVolver" runat="server" CssClass="messageBoxTitle" Text="Se perderán los cambios no guardados. ¿Desea continuar?"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btConfirmVolverSi" runat="server" Text="Si" Width="70px" CssClass="submitButton"
                                        OnClick="btConfirmVolverSi_Click" />
                                </td>
                                <td>
                                    <asp:Button ID="btConfirmVolverNo" runat="server" Text="No" Width="70px" CssClass="submitButton"
                                        OnClick="btConfirmVolverNo_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Confirmar Volver !-->
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Panel ID="pnArchivosEvaluacion" runat="server" HorizontalAlign="Center">
        <table align="center" class="tableControls">
            <tr>
                <td class="tdTitle">
                    <h1>
                        Panel de Archivos</h1>
                </td>
            </tr>
            <tr>
                <td class="tdFirst">
                    <asp:GridView ID="gvArchivosAccionCorrectiva" runat="server" AutoGenerateColumns="False"
                        Width="963px" HorizontalAlign="Center" OnRowCommand="gvArchivosEvaluacion_RowCommand" ShowHeaderWhenEmpty="True">
                        <AlternatingRowStyle BackColor="#E4E4E4" />
                        <Columns>
                            <asp:TemplateField HeaderText="Nombre">
                                <ItemTemplate>
                                    <asp:Label ID="lbNombre" runat="server"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tipo">
                                <ItemTemplate>
                                    <asp:Label ID="lbTipo" runat="server"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tamaño">
                                <ItemTemplate>
                                    <asp:Label ID="lbSize" runat="server"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:TemplateField>
                            <asp:ButtonField ButtonType="Image" CommandName="RemoveArchivo" HeaderText="Eliminar"
                                ImageUrl="~/Images/Icon/remove_file.png">
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:ButtonField>
                        </Columns>
                        <HeaderStyle BackColor="#666666" ForeColor="White" />
                        <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <EmptyDataTemplate>
                            <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                            <asp:Label ID="lbEmptyList" runat="server" Text="  No se han cargado archivos"
                                Font-Size="14px"></asp:Label>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:FileUpload ID="fuArchivo" runat="server" Width="500" multiple="true" />
                </td>
            </tr>
            <tr>
                <td class="tdLast" align="center">
                    <asp:ImageButton ID="ibAddArchivo" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/Button/attachment.png"
                        CssClass="submitAttachFile" OnClick="ibAddArchivo_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <br />
    <asp:Panel ID="Panel1" runat="server">
        <table align="center">
            <tr>
                <td>
                    <asp:ImageButton ID="ibVolver" runat="server" OnClick="ibVolver_Click" CssClass="submitPanelButton"
                        ImageUrl="~/Images/Button/back.png" />
                </td>
                <td>
                    <asp:ImageButton ID="ibRegistrarAccionCorrectiva" runat="server" CssClass="submitPanelButton"
                        ImageUrl="~/Images/Button/save.png" OnClick="ibRegistrarAccionCorrectiva_Click" />
                </td>
            </tr>
        </table>
        <div align="left" style="color: #FF0000; text-align: center">
            <asp:Literal ID="ltSummary" runat="server"></asp:Literal>
        </div>
    </asp:Panel>
</asp:Content>
