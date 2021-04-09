using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VentasSharp2021
{
    public partial class frmFacturacionVentas : Form


    {

        //lo que se define aqui es publico para todos los elementos de la clase
        public int fila;
        public object numero, cliente, fechaFactura, fechaPago, formaPago; //sp_InsertarMaestroFactura  
        public object producto, cantidad, valor;  //sp_InsertarDetalleFacturaProducto
        public int j;


        MySqlConnection conexion = new MySqlConnection("server=localhost;database=almacenventas;uid=root;password=");

       
       
        public frmFacturacionVentas()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void frmFacturacionVentas_Load(object sender, EventArgs e)
        {
            cargarClientes();
            cargarFormaPago();
            cargarProductos();


            //simulacion de autoincremental en la factura
            DataTable tablaConsecutivo = new DataTable();
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_UltimaFactura", conexion);
            conexion.Open();
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;
            adaptador.Fill(tablaConsecutivo);
            //almacenar el utimo consecutivo
            DataRow ultimaFactura = tablaConsecutivo.Rows[0];
            //variable paraconsecutivo
            int consecutivo = Convert.ToInt32(ultimaFactura["numero"].ToString());
            //llevar var a cuadro de texto
            txtNumeroFactura.Text = Convert.ToString(consecutivo + 1);
            conexion.Close();


        }

        private void buttonInsertarFactura_Click(object sender, EventArgs e)
        {



            //INSERCION DE ENCABEZADO

            //datatable
            DataTable tablaEncabezado = new DataTable();

            //adaptador
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_insertarMaestroFactura", conexion);
            conexion.Open();


            //cuadar parametros
            numero = txtNumeroFactura.Text;
            cliente = comboBoxCliente.SelectedValue.ToString();
            fechaFactura = dateTimePickerFcreacion.Value.Year.ToString() + '-' + dateTimePickerFcreacion.Value.Month.ToString() + '-' + dateTimePickerFcreacion.Value.Day.ToString();

            fechaPago = dateTimePickerFVencimiento.Value.Year.ToString() + '-' + dateTimePickerFVencimiento.Value.Month.ToString() + '-' + dateTimePickerFVencimiento.Value.Day.ToString();
            formaPago = comboBoxFPago.SelectedValue.ToString();



            //decir que tipo de comando se va a ejecutar
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;

            adaptador.SelectCommand.Parameters.Add("p_numero", MySqlDbType.Int32).Value = numero;
            adaptador.SelectCommand.Parameters.Add("p_cliente", MySqlDbType.String).Value = cliente;
            adaptador.SelectCommand.Parameters.Add("p_fechafact", MySqlDbType.Date).Value = fechaFactura;
            adaptador.SelectCommand.Parameters.Add("p_fechapago", MySqlDbType.Date).Value = fechaPago;
            adaptador.SelectCommand.Parameters.Add("p_formapago", MySqlDbType.String).Value = formaPago;

            adaptador.Fill(tablaEncabezado);
            conexion.Close();

            //INSERCION DETALLE FACTURA

            fila = dataGridView1.Rows.Count;
          

            for (j=0; j < fila; j++) {


                //datatable
                DataTable tablaDetalle = new DataTable();

                //adaptador
                MySqlDataAdapter adaptador2 = new MySqlDataAdapter("sp_InsertarDetalleFacturaProducto", conexion);
                conexion.Open();


                //cuadar parametros
              
                producto = dataGridView1.Rows[j].Cells[0].Value;
                cantidad = dataGridView1.Rows[j].Cells[2].Value;
                valor = dataGridView1.Rows[j].Cells[3].Value;




                //decir que tipo de comando se va a ejecutar
                adaptador2.SelectCommand.CommandType = CommandType.StoredProcedure;

                adaptador2.SelectCommand.Parameters.Add("p_numfactura", MySqlDbType.Int32).Value = numero;
                adaptador2.SelectCommand.Parameters.Add("p_codproducto", MySqlDbType.String).Value = producto;
                adaptador2.SelectCommand.Parameters.Add("p_cantidad", MySqlDbType.Int32).Value = cantidad;
                adaptador2.SelectCommand.Parameters.Add("p_valor", MySqlDbType.Int32).Value = valor;

                adaptador2.Fill(tablaDetalle);

             
                conexion.Close();

            }
            MessageBox.Show("Factura registrada !");

           


        }

        private void textBoxTotal_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void txtNumeroFactura_TextChanged(object sender, EventArgs e)
        {
            txtNumeroFactura.Enabled = false;
        }

        public void cargarClientes() {
            //datable
            DataTable tablaClientes = new DataTable();


            //adaptador
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_ListarClientes", conexion);

            conexion.Open();

            //decir que tipo de comando se va a ejecutar
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;

            //ejecutar adaptador
            adaptador.Fill(tablaClientes);

            //validar si trae registros
            if (tablaClientes.Rows.Count > 0)
            {
                //imprimir en dgv
                comboBoxCliente.DataSource = tablaClientes;

                //ocultar unidad medida
                comboBoxCliente.DisplayMember = "Cliente";
                comboBoxCliente.ValueMember = "Documento Identidad";
            }
            else
            {
                MessageBox.Show("La tabla de clientes esta vacia", "Alerta", MessageBoxButtons.OK);
            }


            //cerra conexion
            conexion.Close();
        }
        public void cargarFormaPago() {
            //datable
            DataTable tablaFormaPago = new DataTable();


            //adaptador
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_ListarFormasPago  ", conexion);

            conexion.Open();

            //decir que tipo de comando se va a ejecutar
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;

            //ejecutar adaptador
            adaptador.Fill(tablaFormaPago);

            //validar si trae registros
            if (tablaFormaPago.Rows.Count > 0)
            {
                //imprimir en dgv
                comboBoxFPago.DataSource = tablaFormaPago;

                //ocultar unidad medida
                comboBoxFPago.DisplayMember = "descripcion";
                comboBoxFPago.ValueMember = "codigo";
            }
            else
            {
                MessageBox.Show("La tabla forma de pago esta vacia", "Alerta", MessageBoxButtons.OK);
            }


            //cerra conexion
            conexion.Close();
        }
        public void cargarProductos()
        {   //datable
            DataTable tablaProductos = new DataTable();


            //adaptador
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_ListarProductosOrdenadosNombre", conexion);

            conexion.Open();

            //decir que tipo de comando se va a ejecutar
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;

            //ejecutar adaptador
            adaptador.Fill(tablaProductos);

            //validar si trae registros
            if (tablaProductos.Rows.Count > 0)
            {
                //imprimir en dgv
                comboBoxProducto.DataSource = tablaProductos;

                //ocultar unidad medida
                comboBoxProducto.DisplayMember = "nombre";
                comboBoxProducto.ValueMember = "codigo";
            }
            else
            {
                MessageBox.Show("La tabla de productos esta vacia", "Alerta", MessageBoxButtons.OK);
            }


            //cerra conexion
            conexion.Close();
        }
        public void calcularTotal()
        {
            int total=0;

            fila = dataGridView1.Rows.Count;
            for(j= 0; j <fila; j++)
            {
                total = total + Convert.ToInt32(dataGridView1.Rows[j].Cells[4].Value);
            }
            textBoxTotal.Text = Convert.ToString(total);
        }

        private void comboBoxProducto_DropDownClosed(object sender, EventArgs e)
        {
            //sacar el costo de la tabla producto

            //datatable
            DataTable tablaPrecioProducto = new DataTable();
            //adaptador
            MySqlDataAdapter adaptador = new MySqlDataAdapter("sp_PrecioProducto", conexion);
            conexion.Open();
            //decir que tipo de comando se va a ejecutar
            adaptador.SelectCommand.CommandType = CommandType.StoredProcedure;
            //guardar parametro de sp
            String codigoProducto = comboBoxProducto.SelectedValue.ToString();
            adaptador.SelectCommand.Parameters.Add("p_codigo", MySqlDbType.String).Value = codigoProducto;
            //ejecutar adaptador
            adaptador.Fill(tablaPrecioProducto);

            //agregarlo al datarow y despues a variable
            DataRow precioProducto = tablaPrecioProducto.Rows[0];

            textBoxValorUnitario.Text = precioProducto["costo"].ToString();

            textBoxCantidad.Text = Convert.ToString(1);

            textBoxCantidad.Focus();
            conexion.Close();


        }

        private void buttonAgregar_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add();

            dataGridView1.Rows[fila].Cells[0].Value = comboBoxProducto.SelectedValue.ToString();
            dataGridView1.Rows[fila].Cells[1].Value = comboBoxProducto.Text;
            dataGridView1.Rows[fila].Cells[2].Value = textBoxCantidad.Text;
            dataGridView1.Rows[fila].Cells[3].Value = textBoxValorUnitario.Text;
            dataGridView1.Rows[fila].Cells[4].Value = Convert.ToInt32(textBoxCantidad.Text) * Convert.ToInt32(textBoxValorUnitario.Text);            //contador para agregar producto en la siguiente fila
                fila = fila + 1;

            calcularTotal();

        }
    }
}
