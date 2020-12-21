<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Usuarios.aspx.cs" Inherits="NCCSAN.Administracion.Usuarios" %>

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Lista de Usuarios</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnUsuarios" runat="server" HorizontalAlign="Center">
                <table class="tableControls" align="center">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label22" runat="server" Text="RUT"></asp:Label>
                            <asp:TextBox ID="txtPersonaRut" runat="server" AutoPostBack="True"></asp:TextBox>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label1" runat="server" Text="Nombre Usuario"></asp:Label>
                            <asp:TextBox ID="txtUsuarioNombre" runat="server" AutoPostBack="True"></asp:TextBox>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label16" runat="server" Text="Nombre Persona"></asp:Label>
                            <asp:TextBox ID="txtPersonaNombre" runat="server" AutoPostBack="True"></asp:TextBox>
                        </td>
                        <td class="tdUnique">
                            <asp:Button ID="btUsuarioBuscar" runat="server" Text="Buscar" OnClick="btPersonaBuscar_Click" />
                            <asp:Button ID="btUsuarioLimpiar" runat="server" Text="Limpiar" OnClick="btPersonaLimpiar_Click" />
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
                                        <FooterStyle HorizontalAlign="Center" />
                                        <FooterTemplate>
                                            <asp:Button ID="btAddUsuario" runat="server" Text="+ Agregar" CommandName="AgregarUsuario" />
                                        </FooterTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="nombre_rol" HeaderText="Rol" SortExpression="nombre_rol">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="email" HeaderText="Email" NullDisplayText=" ">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:ButtonField HeaderText="Editar" ButtonType="Image" ImageUrl="~/Images/Icon/edit.png"
                                        ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" Text="Editar"
                                        CommandName="EditarUsuario">
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
                                
                                SelectCommand="SELECT u.usuario, u.nombre_rol, p.email, p.nombre FROM Usuario u, Persona p WHERE (u.nombre_rol&lt;&gt;'Desarrollador') AND  (u.rut_persona=p.rut) AND ((p.id_centro=@id_centro) OR (p.id_centro=@id_rsp)) AND ((p.rut LIKE '%' + @rut + '%') AND (u.usuario LIKE '%' + @nombre_usuario + '%') AND (p.nombre LIKE '%' + @nombre_persona + '%')) ORDER BY u.usuario ASC">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                                    <asp:Parameter DefaultValue="RSP" Name="id_rsp" />
                                    <asp:ControlParameter ControlID="txtPersonaRut" ConvertEmptyStringToNull="False"
                                        Name="rut" PropertyName="Text" Type="String" />
                                    <asp:ControlParameter ControlID="txtUsuarioNombre" ConvertEmptyStringToNull="False"
                                        Name="nombre_usuario" PropertyName="Text" Type="String" />
                                    <asp:ControlParameter ControlID="txtPersonaNombre" ConvertEmptyStringToNull="False"
                                        Name="nombre_persona" PropertyName="Text" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                            <div align="left" style="text-align: center">
                                <asp:Label ID="lbPersonasCount" runat="server"></asp:Label>
                            </div>
                            <br />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
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
                            <asp:Label ID="Label11" runat="server" Text="Persona"></asp:Label>
                            &nbsp;<asp:Label ID="Label6" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtNombrePersonaAgregar" runat="server" ReadOnly="True" Width="230px"></asp:TextBox>
                            <asp:Button ID="btSeleccionarPersona" runat="server" Text="Seleccionar" OnClick="btSeleccionarPersona_Click" />
                            <asp:HiddenField ID="hfRutPersonaAgregar" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label14" runat="server" Text="Usuario"></asp:Label>
                            &nbsp;<asp:Label ID="Label8" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtUsuarioAgregar" runat="server" MaxLength="30"></asp:TextBox>
                            <asp:Button ID="btGenerarUsuario" runat="server" Text="Generar" OnClick="btGenerarUsuario_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label15" runat="server" Text="Rol"></asp:Label>
                            &nbsp;<asp:Label ID="Label10" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlRolAgregar" runat="server" DataSourceID="SDSRol" DataTextField="nombre"
                                DataValueField="nombre" OnDataBound="ddlRolAgregar_DataBound">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSRol" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                
                                
                                
                                SelectCommand="SELECT r.nombre FROM Rol r WHERE (r.nombre &lt;&gt; 'Desarrollador') AND (r.nombre &lt;&gt; 'Administrador') AND (r.nombre &lt;&gt; 'Cliente') AND (r.nombre &lt;&gt; 'RSP')">
                            </asp:SqlDataSource>
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
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label9" runat="server" Text="Email"></asp:Label>
                            &nbsp;<asp:Label ID="Label17" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtEmailAgregar" runat="server" MaxLength="70" Width="230px"></asp:TextBox>
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
    <!-- Panel Editar Usuario -->
    <asp:HiddenField ID="hfEditarUsuario" runat="server" />
    <asp:ModalPopupExtender ID="hfEditarUsuario_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
        DynamicServicePath="" Enabled="True" PopupControlID="upEditarUsuario" TargetControlID="hfEditarUsuario">
    </asp:ModalPopupExtender>
    <asp:UpdatePanel ID="upEditarUsuario" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnEditarUsuario" runat="server" CssClass="Popup_frontground">
                <table class="tableControls">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label2" runat="server" Text="Persona"></asp:Label>
                            &nbsp;<asp:Label ID="Label18" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtNombrePersonaEditar" runat="server" Enabled="False" ReadOnly="True"
                                Width="230px"></asp:TextBox>
                                <asp:HiddenField ID="hfRutPersonaEditar" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label3" runat="server" Text="Usuario"></asp:Label>
                            &nbsp;<asp:Label ID="Label19" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtUsuarioEditar" runat="server" Enabled="False" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label4" runat="server" Text="Rol"></asp:Label>
                            &nbsp;<asp:Label ID="Label20" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlRolEditar" runat="server" DataSourceID="SDSRol" DataTextField="nombre"
                                DataValueField="nombre" OnDataBound="ddlRolEditar_DataBound">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label5" runat="server" Text="Email"></asp:Label>
                            &nbsp;<asp:Label ID="Label21" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtEmailEditar" runat="server" MaxLength="70" Width="230px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <div align="left" style="text-align: center">
                    <asp:Literal ID="ltSummaryEditar" runat="server"></asp:Literal>
                </div>
                <br />
                <table align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btAceptarEdicionUsuario" runat="server" Text="Registrar" CssClass="submitButton"
                                OnClick="btAceptarEdicionUsuario_Click" />
                            <asp:Button ID="btCancelarEdicionUsuario" runat="server" Text="Cancelar" CssClass="submitButton"
                                OnClick="btCancelarEdicionUsuario_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Editar Usuario !-->
    <!-- Panel Buscar Persona -->
    <asp:HiddenField ID="hfBuscarPersona" runat="server" />
    <asp:ModalPopupExtender ID="hfBuscarPersona_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
        DynamicServicePath="" Enabled="True" PopupControlID="upBuscarPersona" TargetControlID="hfBuscarPersona">
    </asp:ModalPopupExtender>
    <asp:UpdatePanel ID="upBuscarPersona" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnBuscarPersona" runat="server" CssClass="Popup_frontground">
                <table class="tableControls">
                    <tr>
                        <td colspan="2" class="tdTitle">
                            <h1>
                                Buscar persona</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label12" runat="server" Text="Apellido"></asp:Label>
                            <asp:TextBox ID="txtBuscarPersonaApellido" runat="server" AutoPostBack="True"></asp:TextBox>
                            <asp:Button ID="btBuscarPersona" runat="server" Text="Buscar" />
                            <asp:Button ID="btBuscarPersonaLimpiar" runat="server" Text="Limpiar" OnClick="btBuscarPersonaLimpiar_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:GridView ID="gvBuscarPersonas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                DataKeyNames="rut" DataSourceID="SDSBuscarPersonas" HorizontalAlign="Center"
                                OnRowCommand="gvBuscarPersonas_RowCommand" Width="1010px">
                                <AlternatingRowStyle BackColor="#EDEDED" />
                                <Columns>
                                    <asp:ButtonField HeaderText="Seleccionar" ButtonType="Image" ImageUrl="~/Images/Icon/add_user.png"
                                        CommandName="SetPersona" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                    <asp:BoundField DataField="rut" HeaderText="RUT" ReadOnly="True" SortExpression="rut" />
                                    <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" />
                                    <asp:BoundField DataField="edad" HeaderText="Edad" ReadOnly="True" SortExpression="edad">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="cargo" HeaderText="Cargo" SortExpression="cargo">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="antiguedad" HeaderText="Antigüedad" ReadOnly="True" SortExpression="antiguedad">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                </Columns>
                                <HeaderStyle BackColor="#666666" ForeColor="White" />
                                <PagerSettings FirstPageText="" LastPageText="" Mode="NextPreviousFirstLast" NextPageText=""
                                    PreviousPageText="" FirstPageImageUrl="~/Images/Icon/first.png" LastPageImageUrl="~/Images/Icon/last.png"
                                    NextPageImageUrl="~/Images/Icon/next.png" PreviousPageImageUrl="~/Images/Icon/previous.png" />
                                <PagerStyle HorizontalAlign="Center" />
                            </asp:GridView>
                            <asp:SqlDataSource ID="SDSBuscarPersonas" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                
                                
                                SelectCommand="SELECT p.rut, p.nombre, DATEDIFF(YEAR, p.fecha_nacimiento, SYSDATETIME()) AS edad, p.cargo, DATEDIFF(YEAR, p.fecha_ingreso, SYSDATETIME()) AS antiguedad FROM Persona p WHERE (p.nombre LIKE '%' + @nombre + '%') AND ((p.id_centro=@id_centro) OR (p.id_centro=@id_rsp)) AND (p.fecha_retiro IS NULL) AND (p.nombre_clasificacionpersona&lt;&gt;'Clientes') ORDER BY p.nombre ASC">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtBuscarPersonaApellido" Name="nombre" PropertyName="Text" />
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                                    <asp:Parameter DefaultValue="RSP" Name="id_rsp" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                            <div align="left" style="text-align: center">
                                <asp:Literal ID="ltBuscarPersonaSummary" runat="server"></asp:Literal>
                            </div>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btBuscarPersonaCancelar" runat="server" Text="Cancelar" OnClick="btBuscarPersonaCancelar_Click"
                                CssClass="submitButton" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Buscar Persona !-->
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
