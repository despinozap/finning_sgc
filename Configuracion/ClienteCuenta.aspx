<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ClienteCuenta.aspx.cs" Inherits="NCCSAN.Administracion.CentroClienteUsuarios" %>

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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfNombreCliente" runat="server" />
    <div class="divTituloForm">
        <h1>
            Cuenta de acceso</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnCuentaAcceso" runat="server" HorizontalAlign="Center">
                <table class="tableControls" align="center">
                <tr>
                        <td class="tdTitle">
                            <h1>
                                Configuración de Cuenta</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" colspan="4">
                            <asp:GridView ID="gvCuentas" runat="server" AutoGenerateColumns="False"
                                DataKeyNames="rut" DataSourceID="SDSCuentas" HorizontalAlign="Center" OnRowCommand="gvCuentas_RowCommand"
                                Width="1010px" ShowHeaderWhenEmpty="True">
                                <AlternatingRowStyle CssClass="rowDataAlt" />
                                <Columns>
                                    <asp:BoundField DataField="nombre" HeaderText="Nombre de Cuenta" ReadOnly="True"
                                        SortExpression="nombre"></asp:BoundField>
                                    <asp:BoundField DataField="nombre_cliente" HeaderText="Cliente" SortExpression="nombre_cliente">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="email" HeaderText="Email" SortExpression="email">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="fecha_ingreso" HeaderText="Fecha de Ingreso" SortExpression="fecha_ingreso">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:ButtonField HeaderText="Editar" ButtonType="Image" ImageUrl="~/Images/Icon/edit.png"
                                        ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" Text="Editar"
                                        CommandName="EditarCuenta">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                    <asp:ButtonField ButtonType="Image" HeaderText="Eliminar" ImageUrl="~/Images/Icon/remove_user.png"
                                        Text="Eliminar" CommandName="EliminarCuenta">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                </Columns>
                                <HeaderStyle BackColor="#666666" ForeColor="White" />
                                <RowStyle CssClass="rowData" />
                                <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <EmptyDataTemplate>
                                    <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                    <asp:Label ID="lbEmptyList" runat="server" Text="  El Cliente no tiene configurada la cuenta de acceso"
                                        Font-Size="14px"></asp:Label>
                                    <br />
                                    <asp:Button ID="btConfigureAccount" runat="server" Text="Configurar" CommandName="ConfigurarCuenta" />
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:SqlDataSource ID="SDSCuentas" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                
                                
                                SelectCommand="SELECT p.rut, p.nombre, cp.nombre_cliente, p.email, CONVERT(VARCHAR(10), fecha_ingreso, 105) AS fecha_ingreso FROM Persona p, ClientePersona cp WHERE (cp.nombre_cliente=@nombre_cliente) AND (p.rut=cp.rut_persona)">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="hfNombreCliente" Name="nombre_cliente" PropertyName="Value" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                            <br />
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table class="tableControls" align="center">
                <tr>
                        <td class="tdTitle">
                            <h1>
                                Lista de Usuarios asociados al Cliente</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" colspan="5">
                            <asp:GridView ID="gvUsuarios" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                DataKeyNames="usuario" DataSourceID="SDSUsuarios" HorizontalAlign="Center" OnRowCommand="gvUsuarios_RowCommand"
                                Width="1010px" ShowFooter="True" ShowHeaderWhenEmpty="True">
                                <AlternatingRowStyle CssClass="rowDataAlt" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Usuario">
                                        <ItemTemplate>
                                            <asp:Label ID="lbUsuario" runat="server" Text='<%#Eval("usuario") %>' Font-Bold="True"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="nombre_rol" HeaderText="Rol" SortExpression="nombre_rol">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:ButtonField HeaderText="Restablecer clave" ButtonType="Image" ImageUrl="~/Images/Icon/undo.png"
                                        ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" Text="Restablecer clave"
                                        CommandName="ResetPassword">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                    <asp:ButtonField ButtonType="Image" HeaderText="Eliminar" ImageUrl="~/Images/Icon/remove_user.png"
                                        Text="Eliminar" CommandName="EliminarUsuario">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                </Columns>
                                <HeaderStyle BackColor="#666666" ForeColor="White" />
                                <PagerSettings FirstPageText="" LastPageText="" Mode="NextPreviousFirstLast" NextPageText=""
                                    PreviousPageText="" FirstPageImageUrl="~/Images/Icon/first.png" LastPageImageUrl="~/Images/Icon/last.png"
                                    NextPageImageUrl="~/Images/Icon/next.png" PreviousPageImageUrl="~/Images/Icon/previous.png" />
                                <PagerStyle HorizontalAlign="Center" />
                                <RowStyle CssClass="rowData" />
                                <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <EmptyDataTemplate>
                                    <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                    <asp:Label ID="lbEmptyList" runat="server" Text="  No se han encontrado registros"
                                        Font-Size="14px"></asp:Label>
                                    <br />
                                    <asp:Button ID="btAddUsuario" runat="server" Text="+ Agregar" CommandName="AgregarUsuario" />
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:SqlDataSource ID="SDSUsuarios" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                
                                
                                SelectCommand="SELECT u.usuario, u.nombre_rol FROM Usuario u, ClientePersona cp WHERE (cp.nombre_cliente=@nombre_cliente) AND (u.rut_persona=cp.rut_persona) ORDER BY u.usuario ASC">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="hfNombreCliente" Name="nombre_cliente" PropertyName="Value" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                            <div align="left" style="text-align: center">
                                <asp:Label ID="lbPersonasCount" runat="server"></asp:Label>
                            </div>
                            <br />
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table align="center">
                    <tr>
                        <td>
                            <asp:ImageButton ID="ibVolver" runat="server" ImageUrl="~/Images/Button/back.png"
                                ImageAlign="AbsMiddle" CssClass="submitPanelButton" OnClick="ibVolver_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Agregar Cuenta -->
    <asp:HiddenField ID="hfAgregarCuenta" runat="server" />
    <asp:ModalPopupExtender ID="hfAgregarCuenta_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
        DynamicServicePath="" Enabled="True" PopupControlID="upAgregarCuenta" TargetControlID="hfAgregarCuenta">
    </asp:ModalPopupExtender>
    <asp:UpdatePanel ID="upAgregarCuenta" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnAgregarCuenta" runat="server" CssClass="Popup_frontground">
                <table class="tableControls">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label8" runat="server" Text="Email"></asp:Label>
                            &nbsp;<asp:Label ID="Label28" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="70" Width="230px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <div align="left" style="text-align: center">
                    <asp:Literal ID="ltSummary" runat="server"></asp:Literal>
                </div>
                <br />
                <table align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btAceptarRegistroCuenta" runat="server" Text="Registrar" CssClass="submitButton"
                                OnClick="btAceptarRegistroCuenta_Click" />
                            <asp:Button ID="btCancelarRegistroCuenta" runat="server" Text="Cancelar" CssClass="submitButton"
                                OnClick="btCancelarRegistroCuenta_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Agregar Cuenta !-->
    <!-- Panel Eliminar Cuenta -->
    <asp:HiddenField ID="hfEliminarCuenta" runat="server" />
    <asp:ModalPopupExtender ID="hfEliminarCuenta_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
        DynamicServicePath="" Enabled="True" PopupControlID="upEliminarCuenta" TargetControlID="hfEliminarCuenta">
    </asp:ModalPopupExtender>
    <asp:UpdatePanel ID="upEliminarCuenta" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnEliminarCuenta" runat="server" CssClass="Popup_frontground">
                <table align="center">
                    <tr>
                        <td>
                            <asp:Label ID="lbMessageEliminarCuenta" runat="server" CssClass="messageBoxMessage"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btEliminarCuenta" runat="server" Text="Aceptar" CssClass="submitButton"
                                OnClick="btEliminarCuenta_Click" />
                            <asp:Button ID="btCancelarEliminarCuenta" runat="server" Text="Cancelar" CssClass="submitButton"
                                OnClick="btCancelarEliminarCuenta_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Eliminar Cuenta !-->
    <!-- Panel Agregar Usuario -->
    <asp:HiddenField ID="hfAgregarUsuario" runat="server" />
    <asp:ModalPopupExtender ID="hfAgregarUsuario_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
        DynamicServicePath="" Enabled="True" PopupControlID="upAgregarUsuario" TargetControlID="hfAgregarUsuario">
    </asp:ModalPopupExtender>
    <asp:UpdatePanel ID="upAgregarUsuario" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnAgregarUsuario" runat="server" CssClass="Popup_frontground">
                <table class="tableControls">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label14" runat="server" Text="Usuario"></asp:Label>
                            &nbsp;<asp:Label ID="Label1" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtUsuarioAgregar" runat="server" MaxLength="30"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label7" runat="server" Text="Clave"></asp:Label>
                            &nbsp;<asp:Label ID="Label13" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtClaveAgregar" runat="server" MaxLength="30"></asp:TextBox>
                            <asp:CheckBox ID="chbDefaultPassword" runat="server" OnCheckedChanged="chbDefaultPassword_CheckedChanged"
                                Text="Utilizar clave por defecto" AutoPostBack="True" />
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <div align="left" style="text-align: center">
                    <asp:Literal ID="ltSummaryAgregar" runat="server"></asp:Literal>
                </div>
                <br />
                <table align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btAceptarRegistroUsuario" runat="server" Text="Registrar" CssClass="submitButton"
                                OnClick="btAceptarRegistroUsuario_Click" />
                            <asp:Button ID="btCancelarRegistroUsuario" runat="server" Text="Cancelar" CssClass="submitButton"
                                OnClick="btCancelarRegistroUsuario_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Agregar Usuario !-->
    <!-- Panel Confirmar Restablecer Password -->
    <asp:HiddenField ID="hfConfirmResetPassword" runat="server" />
    <asp:ModalPopupExtender ID="hfConfirmResetPassword_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
        DynamicServicePath="" Enabled="True" PopupControlID="upConfirmResetPassword" TargetControlID="hfConfirmResetPassword">
    </asp:ModalPopupExtender>
    <asp:UpdatePanel ID="upConfirmResetPassword" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnConfirmResetPassword" runat="server" CssClass="Popup_frontground">
                <table class="tableControls">
                    <tr>
                        <td">
                            <asp:Label ID="Label3" runat="server" CssClass="messageBoxMessage" Text="Se va a restablecer la clave de acceso por defecto (0000). ¿Desea continuar?"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btAceptarResetPassword" runat="server" Text="Aceptar" CssClass="submitButton"
                                OnClick="btAceptarResetPassword_Click" />
                            <asp:Button ID="btCancelarResetPassword" runat="server" Text="Cancelar" CssClass="submitButton"
                                OnClick="btCancelarResetPassword_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Confirmar Restablecer Password !-->
    <!-- Panel Eliminar Usuario -->
    <asp:HiddenField ID="hfEliminarUsuario" runat="server" />
    <asp:ModalPopupExtender ID="hfEliminarUsuario_ModalPopupExtender" runat="server"
        BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" PopupControlID="upEliminarUsuario"
        TargetControlID="hfEliminarUsuario">
    </asp:ModalPopupExtender>
    <asp:UpdatePanel ID="upEliminarUsuario" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnEliminarUsuario" runat="server" CssClass="Popup_frontground">
                <table align="center">
                    <tr>
                        <td>
                            <asp:Label ID="lbMessageEliminarUsuario" runat="server" CssClass="messageBoxMessage"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btEliminarUsuario" runat="server" Text="Aceptar" CssClass="submitButton"
                                OnClick="btEliminarUsuario_Click" />
                            <asp:Button ID="btCancelarEliminarUsuario" runat="server" Text="Cancelar" CssClass="submitButton"
                                OnClick="btCancelarEliminarUsuario_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Eliminar Usuario !-->
</asp:Content>
