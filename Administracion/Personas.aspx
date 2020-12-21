<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Personas.aspx.cs" Inherits="NCCSAN.Administracion.Personas" %>

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
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

            function EndRequestHandler(sender, args) {
                $("#<%= txtFechaNacimiento.ClientID %>").datepicker
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
                        firstDay: 1,
                        changeYear: true,
                        yearRange: '1900:' + (new Date).getFullYear()
                    }
                );

                $("#<%= txtFechaIngreso.ClientID %>").datepicker
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
                        firstDay: 1,
                        changeYear: true,
                        yearRange: '1900:' + (new Date).getFullYear()
                    }
                );


                $("#<%= txtFechaRetiro.ClientID %>").datepicker
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
                        firstDay: 1,
                        changeYear: true,
                        yearRange: '1900:' + (new Date).getFullYear()
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
            Lista de Personas</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnPersonas" runat="server" HorizontalAlign="Center">
                <table class="tableControls" align="center">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label22" runat="server" Text="RUT"></asp:Label>
                            <asp:TextBox ID="txtPersonaRut" runat="server" AutoPostBack="True"></asp:TextBox>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label16" runat="server" Text="Nombre"></asp:Label>
                            <asp:TextBox ID="txtPersonaNombre" runat="server" AutoPostBack="True"></asp:TextBox>
                        </td>
                        <td class="tdUnique">
                            <asp:Label ID="Label34" runat="server" Text="Clasificación"></asp:Label>
                            <asp:DropDownList ID="ddlClasificacionPersonaBuscar" runat="server" DataSourceID="SDSClasificacionPersona"
                                DataTextField="nombre" DataValueField="nombre" AutoPostBack="True" OnDataBound="ddlClasificacionPersonaBuscar_DataBound">
                            </asp:DropDownList>
                        </td>
                        <td class="tdUnique">
                            <asp:Button ID="btPersonaBuscar" runat="server" Text="Buscar" OnClick="btPersonaBuscar_Click" />
                            <asp:Button ID="btPersonaLimpiar" runat="server" Text="Limpiar" OnClick="btPersonaLimpiar_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique" colspan="4">
                            <asp:GridView ID="gvPersonas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                DataKeyNames="rut" DataSourceID="SDSPersonas" HorizontalAlign="Center" OnRowCommand="gvPersonas_RowCommand"
                                Width="1010px" ShowFooter="True" ShowHeaderWhenEmpty="True">
                                <AlternatingRowStyle CssClass="rowDataAlt" />
                                <Columns>
                                    <asp:TemplateField HeaderText="RUT">
                                        <ItemTemplate>
                                            <asp:Label ID="lbRUT" runat="server" Text='<%#Eval("rut") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <FooterTemplate>
                                            <asp:Button ID="btAddPersona" runat="server" Text="+ Agregar" CommandName="AgregarPersona" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Nombre">
                                        <ItemTemplate>
                                            <asp:Label ID="lbNombre" runat="server" Text='<%#Eval("nombre") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="edad" HeaderText="Edad" ReadOnly="True" SortExpression="edad">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="email" HeaderText="Email">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="cargo" HeaderText="Cargo" SortExpression="cargo">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="nombre_clasificacionpersona" HeaderText="Clasificación"
                                        SortExpression="nombre_clasificacionpersona">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="antiguedad" HeaderText="Antigüedad" ReadOnly="True" SortExpression="antiguedad">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:ButtonField HeaderText="Editar" ButtonType="Image" ImageUrl="~/Images/Icon/edit.png"
                                        ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" Text="Editar"
                                        CommandName="EditarPersona">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                    <asp:ButtonField ButtonType="Image" HeaderText="Eliminar" ImageUrl="~/Images/Icon/remove_user.png"
                                        Text="Eliminar" CommandName="EliminarPersona">
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
                                    <asp:Button ID="btAddPersona" runat="server" Text="+ Agregar" CommandName="AgregarPersona" />
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:SqlDataSource ID="SDSPersonas" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT p.rut, p.nombre, DATEDIFF(YEAR, p.fecha_nacimiento, SYSDATETIME()) AS edad, p.cargo, p.nombre_clasificacionpersona, p.email, DATEDIFF(YEAR, p.fecha_ingreso, SYSDATETIME()) AS antiguedad FROM Persona p WHERE ((p.rut LIKE '%' + @rut + '%') AND (p.nombre LIKE '%' + @nombre + '%') AND (nombre_clasificacionpersona LIKE '%' + @nombre_clasificacionpersona + '%')) AND ((p.id_centro=@id_centro) OR (p.id_centro=@id_rsp))  AND (p.nombre_clasificacionpersona&lt;&gt;'Clientes') ORDER BY p.nombre ASC"
                                OnSelected="SDSPersonas_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtPersonaRut" ConvertEmptyStringToNull="False"
                                        Name="rut" PropertyName="Text" Type="String" />
                                    <asp:ControlParameter ControlID="txtPersonaNombre" Name="nombre" PropertyName="Text"
                                        ConvertEmptyStringToNull="False" DefaultValue=" " Type="String" />
                                    <asp:ControlParameter ControlID="ddlClasificacionPersonaBuscar" ConvertEmptyStringToNull="False"
                                        DefaultValue="" Name="nombre_clasificacionpersona" PropertyName="SelectedValue"
                                        Type="String" />
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                                    <asp:Parameter DefaultValue="RSP" Name="id_rsp" />
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
            <!-- Panel Confirmar Transferir Persona-->
            <asp:HiddenField ID="hfConfirmTransferirPersona" runat="server" />
            <asp:ModalPopupExtender ID="hfConfirmTransferirPersona_ModalPopupExtender" runat="server"
                BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" TargetControlID="hfConfirmTransferirPersona"
                PopupControlID="pnConfirmTransferirPersona">
            </asp:ModalPopupExtender>
            <asp:UpdatePanel ID="upConfirmTransferirPersona" runat="server" style="width: 100%"
                UpdateMode="Conditional">
                <ContentTemplate>
                    <br />
                    <br />
                    <asp:Panel ID="pnConfirmTransferirPersona" runat="server" CssClass="Popup_frontground"
                        HorizontalAlign="Center" Width="100%">
                        <asp:Label ID="lbMessageConfirmTransferirPersona" runat="server" CssClass="messageBoxTitle"
                            Text="Se va a registrar la persona en un Centro externo y por tanto no aparecerá en la lista de personas de tú Centro. ¿Deseas continuar?"></asp:Label>
                        <br />
                        <br />
                        <table align="center">
                            <tr>
                                <td>
                                    <asp:Button ID="btConfirmTransferirPersonaSi" runat="server" Text="Si" Width="70px"
                                        CssClass="submitButton" OnClick="btConfirmTransferirPersonaSi_Click" />
                                </td>
                                <td>
                                    <asp:Button ID="btConfirmTransferirPersonaNo" runat="server" Text="No" Width="70px"
                                        CssClass="submitButton" OnClick="btConfirmTransferirPersonaNo_Click" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- Panel Confirmar Transferir Persona!-->
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Agregar Persona -->
    <asp:HiddenField ID="hfAgregarPersona" runat="server" />
    <asp:ModalPopupExtender ID="hfAgregarPersona_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
        DynamicServicePath="" Enabled="True" PopupControlID="upAgregarPersona" TargetControlID="hfAgregarPersona">
    </asp:ModalPopupExtender>
    <asp:UpdatePanel ID="upAgregarPersona" runat="server">
        <ContentTemplate>
            <asp:Panel ID="pnAgregarPersona" runat="server" CssClass="Popup_frontground">
                <table class="tableControls">
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="lbRUT" runat="server" Text="RUT"></asp:Label>
                            &nbsp;<asp:Label ID="Label17" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtRUT" runat="server"></asp:TextBox>
                            <asp:Label ID="Label1" runat="server" Text="(Ej: 12345678-9)"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label14" runat="server" Text="Nombres"></asp:Label>
                            &nbsp;<asp:Label ID="Label18" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtNombres" runat="server" MaxLength="55"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label15" runat="server" Text="Apellidos"></asp:Label>
                            &nbsp;<asp:Label ID="Label19" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtApellidos" runat="server" MaxLength="55"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label2" runat="server" Text="Fecha de nacimiento"></asp:Label>
                            &nbsp;<asp:Label ID="Label20" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtFechaNacimiento" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label3" runat="server" Text="Sexo"></asp:Label>
                            &nbsp;<asp:Label ID="Label21" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlSexo" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label32" runat="server" Text="Clasificación"></asp:Label>
                            &nbsp;<asp:Label ID="Label33" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlClasificacionPersona" runat="server" DataSourceID="SDSClasificacionPersona"
                                DataTextField="nombre" DataValueField="nombre" 
                                OnDataBound="ddlClasificacionPersona_DataBound" AutoPostBack="True" 
                                onselectedindexchanged="ddlClasificacionPersona_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSClasificacionPersona" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                
                                SelectCommand="SELECT [nombre] FROM [ClasificacionPersona] WHERE ([nombre] &lt;&gt; @nombre)">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="Clientes" Name="nombre" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label4" runat="server" Text="ID Empleado"></asp:Label>
                            &nbsp;<asp:Label ID="Label40" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtIDEmpleado" runat="server" MaxLength="30"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label30" runat="server" Text="Centro"></asp:Label>
                            &nbsp;<asp:Label ID="lbAsteriskCentro" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlCentro" runat="server" AutoPostBack="True" DataSourceID="SDSCentros"
                                DataTextField="nombre" DataValueField="id" OnDataBound="ddlCentro_DataBound"
                                OnSelectedIndexChanged="ddlCentro_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSCentros" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                
                                SelectCommand="SELECT [id], [nombre] FROM [Centro] WHERE ([id] &lt;&gt; @id_ext) AND ([id] &lt;&gt; @id_rsp)">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="EXT" Name="id_ext" Type="String" />
                                    <asp:Parameter DefaultValue="RSP" Name="id_rsp" Type="String" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label5" runat="server" Text="Store"></asp:Label>
                            &nbsp;<asp:Label ID="lbAsteriskStore" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:DropDownList ID="ddlStore" runat="server" DataSourceID="SDSStores" DataTextField="id"
                                DataValueField="id" OnDataBound="ddlStore_DataBound">
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SDSStores" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT [id] FROM [Store]"></asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label6" runat="server" Text="Lob"></asp:Label>
                            &nbsp;<asp:Label ID="lbAsteriskLob" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtLob" runat="server" MaxLength="10"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label7" runat="server" Text="Cargo"></asp:Label>
                            &nbsp;<asp:Label ID="Label41" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtCargo" runat="server" MaxLength="70" Width="230px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label11" runat="server" Text="Supervisor"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtNombreSupervisor" runat="server" Enabled="False" ReadOnly="True"
                                Width="230px"></asp:TextBox>
                            <asp:Button ID="btSeleccionarSupervisor" runat="server" Text="Seleccionar" OnClick="btSeleccionarSupervisor_Click" />
                            <asp:HiddenField ID="hfRutSupervisor" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label8" runat="server" Text="Email"></asp:Label>
                            &nbsp;<asp:Label ID="Label28" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="70" Width="230px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label9" runat="server" Text="Fecha de ingreso"></asp:Label>
                            &nbsp;<asp:Label ID="Label29" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtFechaIngreso" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label10" runat="server" Text="Fecha de retiro"></asp:Label>
                        </td>
                        <td class="tdUnique">
                            <asp:TextBox ID="txtFechaRetiro" runat="server"></asp:TextBox>
                            <asp:Label ID="Label13" runat="server" Text="(Sólo si está retirado)"></asp:Label>
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
                            <asp:Button ID="btAceptarRegistroPersona" runat="server" Text="Registrar" CssClass="submitButton"
                                OnClick="btAceptarRegistroPersona_Click" />
                            <asp:Button ID="btCancelarRegistroPersona" runat="server" Text="Cancelar" CssClass="submitButton"
                                OnClick="btCancelarRegistroPersona_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Agregar Persona !-->
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
                                        CommandName="SetSupervisor" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
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
                                SelectCommand="SELECT p.rut, p.nombre, DATEDIFF(YEAR, p.fecha_nacimiento, SYSDATETIME()) AS edad, p.cargo, DATEDIFF(YEAR, p.fecha_ingreso, SYSDATETIME()) AS antiguedad FROM Persona p WHERE (p.nombre LIKE '%' + @nombre + '%') AND (p.id_centro=@centro) AND (p.fecha_retiro IS NULL) AND (p.nombre_clasificacionpersona&lt;&gt;'Clientes') ORDER BY p.nombre ASC"
                                OnSelected="SDSBuscarPersonas_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtBuscarPersonaApellido" Name="nombre" PropertyName="Text" />
                                    <asp:ControlParameter ControlID="ddlCentro" Name="centro" PropertyName="SelectedValue" />
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
    <!-- Panel Eliminar Persona -->
    <asp:HiddenField ID="hfEliminarPersona" runat="server" />
    <asp:ModalPopupExtender ID="hfEliminarPersona_ModalPopupExtender" runat="server"
        BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" PopupControlID="upEliminarPersona"
        TargetControlID="hfEliminarPersona">
    </asp:ModalPopupExtender>
    <asp:UpdatePanel ID="upEliminarPersona" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnEliminarPersona" runat="server" CssClass="Popup_frontground">
                <table align="center">
                    <tr>
                        <td>
                            <asp:Label ID="lbMessageEliminarPersona" runat="server" CssClass="messageBoxMessage"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table align="center">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btEliminarPersona" runat="server" Text="Aceptar" CssClass="submitButton"
                                OnClick="btEliminarPersona_Click" />
                            <asp:Button ID="btCancelarEliminarPersona" runat="server" Text="Cancelar" CssClass="submitButton"
                                OnClick="btCancelarEliminarPersona_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- Panel Eliminar Persona !-->
</asp:Content>
