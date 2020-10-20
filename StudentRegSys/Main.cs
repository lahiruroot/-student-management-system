using MaterialSkin.Controls;
using System.Data.SqlClient;
using System.Windows.Forms;
using System;
using System.Globalization;
using System.Data;
using System.Collections.Generic;
using MaterialSkin;



namespace StudentRegSys
{
    public partial class Main : MaterialForm

    {
        private int Id;
        private SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\SUDB.mdf; Integrated Security = True; Connect Timeout = 30");
        private SqlCommand cmd;
        private List<string> modulelist = new List<string>();

        public Main(int user) //This section is using for generate automatic user ID
        {
            InitializeComponent();
            Id = user;
            modulelist.Add("*");
            
        }
        public void autogen()
        {
            string num = "0123456789";
            int len = num.Length;
            string otp = string.Empty;
            int otpdigit = 6;
            string finaldigit;

            int getindex;

            for (int i = 0; i < otpdigit; i++)
            {
                do
                {
                    getindex = new Random().Next(0, len);
                    finaldigit = num.ToCharArray()[getindex].ToString();
                }
                while (otp.IndexOf(finaldigit) != -1);
                otp += finaldigit;

            }

            txtSReg.Text = (otp);
            txtPReg.Text = (otp);
        }
        //User ID section end......
        private void Main_Load(object sender, System.EventArgs e)
        {
            btnSDelete.AutoSize = false;
            btnSSave.AutoSize = false;
            btnPDelete.AutoSize = false;
            btnPModules.AutoSize = false;
            btnPSave.AutoSize = false;
            btnMDelete.AutoSize = false;
            btnMSave.AutoSize = false;
            btnChange.AutoSize = false;
            btnSearch.AutoSize = false;
            btnPassword.AutoSize = false;
            btnUsername.AutoSize = false;
            btnSDelete.Width = 150;
            btnSSave.Width = 150;
            btnPDelete.Width = 150;
            btnPSave.Width = 150;
            btnPModules.Width = 200;
            btnMDelete.Width = 150;
            btnMSave.Width = 150;
            btnChange.Width = 150;
            btnSearch.Width = 150;
            btnPassword.Width = 150;
            btnUsername.Width = 150;
            try
            {
                cmd = new SqlCommand("select StartDate,EndDate from General where Id=1", con);    //Student intake creating this.....
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    dtpBegins.Value = DateTime.ParseExact(reader["StartDate"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    dtpEnds.Value = DateTime.ParseExact(reader["EndDate"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Retrieving data from Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            con.Close();
        }

        #region General
        private void btnUsername_Click(object sender, System.EventArgs e)
        {
            bool verify = true;

            if (CheckUser(txtUsername.Text))
            {
                verify = false;
                MessageBox.Show("Username not available, Try another one", "Admin Setting - Changing Username", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (CheckUsername(txtUsername.Text))
            {
                verify = false;
                MessageBox.Show("Invalid Username.\n#Rules\n1. Username should be longer than 3 characters.\n2. Username cannot have white spaces.", "Changing Username", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            if (verify) { 
                try
                {
                    SqlCommand cmd = new SqlCommand("update Auth set Username='" + txtUsername.Text + "' where Id=" + Id, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Username Change Successfull", "Admin Setting - Changing Username", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Admin Setting - Changing Username", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                con.Close();
            }

        }
        private void btnPassword_Click(object sender, EventArgs e)
        {
            string pass="";
            try
            {
                cmd = new SqlCommand("select Password from Auth where Id=" + Id, con);
                con.Open();
                pass = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Changing Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            con.Close();
            if (txtCurrent.Text != pass)
                MessageBox.Show("The current password is incorrect", "Admin Setting - Changing Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else if (txtConfirm.Text != txtNew.Text)
                MessageBox.Show("Passwords does not match", "Admin Setting - Changing Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else if (txtNew.TextLength < 6)
                MessageBox.Show("Passwords should be longer than 6 characters", "Admin Setting - Changing Password", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                SqlCommand cmd = new SqlCommand("update Auth set Password='" + txtNew.Text + "' where Id=" + Id, con);
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Password Change Successfull", "Admin Setting - Changing Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Admin Setting - Changing Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                con.Close();
            }
        }
        private void btnChange_Click(object sender, EventArgs e)
        {
            if (dtpBegins.Value > dtpEnds.Value)
                MessageBox.Show("Invalid Dates. Please check the dates and try again.", "Registration Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("update General set StartDate='" + dtpBegins.Value.Date.ToString("dd-MM-yyyy") + "',EndDate='" + dtpEnds.Value.Date.ToString("dd-MM-yyyy") + "' where Id=1", con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Registration scheduled", "Registration Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Registration Scheduling", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                con.Close();
            }
        }
   
        #endregion

        #region Functions
        private bool CheckUser(string username)
        {
            int UserExist = 0;
            try
            {
                cmd = new SqlCommand("select count(Id) from Auth where Username='" + username+"'", con);
                con.Open();
                UserExist = (int)cmd.ExecuteScalar();
            }
            catch{}
            con.Close();
            if (UserExist < 1)
                return false;
            else
                return true;
        }
        private bool CheckUsername(string id)
        {
            if (id.Length < 3 || id.Contains(" "))
                return true;
            return false;
        }
        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
        #endregion

        #region Modules
        private void pbxMSearch_Click(object sender, EventArgs e)
        {
                try
                {
                    cmd = new SqlCommand("select count(MId) from Module where MId='" + txtMId.Text + "'", con);
                    con.Open();
                    int recordExists = (int)cmd.ExecuteScalar();
                    con.Close();
                if (recordExists > 0)
                {
                    cmd = new SqlCommand("select Name,Year,Credits,Hours,Cost from Module where MId='" + txtMId.Text + "'", con);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        txtMName.Text = reader["Name"].ToString();
                        txtMYears.Text = reader["Year"].ToString();
                        txtMCredits.Text = reader["Credits"].ToString();
                        txtMHours.Text = reader["Hours"].ToString();
                        txtMCost.Text = reader["Cost"].ToString();
                    }
                }
                else
                {
                    txtMName.Clear();
                    txtMYears.Clear();
                    txtMCredits.Clear();
                    txtMHours.Clear();
                    txtMCost.Clear();
                    MessageBox.Show("Search failed. Record does not exists.", "Modules", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Retrieving data from Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                con.Close();
        }
        private void btnMSave_Click(object sender, EventArgs e)
        {
            cmd = new SqlCommand("select count(MId) from Module where MId='" + txtMId.Text+ "'", con);
            int recordExists;

            try
            {
                float credits;

                if(txtMId.TextLength< 1 || txtMId.Text.Contains(" "))
                    MessageBox.Show("Invalid Module Id.", "Modules", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if(txtMName.TextLength < 1)
                    MessageBox.Show("Invalid Module Name.", "Modules", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtMYears.TextLength < 1 || !IsDigitsOnly(txtMYears.Text) || int.Parse(txtMYears.Text)<1 || int.Parse(txtMYears.Text)>4 || txtMYears.TextLength < 1)
                    MessageBox.Show("Invalid Year.", "Modules", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtMCredits.TextLength < 1 || !IsDigitsOnly(txtMCredits.Text) || txtMCredits.TextLength < 1)
                    MessageBox.Show("Invalid number of Credits.", "Modules", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtMHours.TextLength < 1 || !IsDigitsOnly(txtMHours.Text) || txtMHours.TextLength < 1)
                    MessageBox.Show("Invalid number of Hours.", "Modules", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtMCost.TextLength < 1 || !float.TryParse(txtMCost.Text,out credits) || txtMCost.TextLength < 1)
                    MessageBox.Show("Invalid number of Credits.", "Modules", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else {
                    con.Open();
                    recordExists = (int)cmd.ExecuteScalar();
                    con.Close();
                    if (recordExists < 1)
                    {
                        cmd = new SqlCommand("insert into Module values('" + txtMId.Text + "','"+txtMName.Text+"',"+txtMYears.Text+","+txtMCredits.Text+","+txtMHours.Text+","+txtMCost.Text+")", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("New module added to the system.", "Modules", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("update Module set Name='" + txtMName.Text + "',Year=" + txtMYears.Text +",Credits="+ txtMCredits.Text +",Hours="+ txtMHours.Text+",Cost="+ txtMCost.Text + " where MId='"+txtMId.Text+"'", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Module '"+ txtMId.Text+"' Updated.", "Modules", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Modules", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnMDelete_Click(object sender, EventArgs e)
        {
            try
            {
                cmd = new SqlCommand("delete from Module where MId='" + txtMId.Text + "'", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                txtMName.Clear();
                txtMYears.Clear();
                txtMCredits.Clear();
                txtMHours.Clear();
                txtMCost.Clear();
                MessageBox.Show("Module '" + txtMId.Text+"' was removed.", "Modules", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMId.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Modules", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion Modules

        #region Tools
        private void btnReset_Click(object sender, EventArgs e)
        {
           
        }
        private void cmbxSearchTable_SelectedIndexChanged(object sender, EventArgs e) // This is Module management Section..
        {
            if (cmbxSearchTable.SelectedItem.ToString() == "Modules")
            {
                cmbxSearchBy.Items.Clear();
                cmbxSearchBy.Items.Add("Module Id");
                cmbxSearchBy.Items.Add("Year");
                cmbxSearchBy.Items.Add("Credits");
            }
            else if(cmbxSearchTable.SelectedItem.ToString() == "Students")
            {
                cmbxSearchBy.Items.Clear();
                cmbxSearchBy.Items.Add("Registraion Number");
                cmbxSearchBy.Items.Add("First Name");
                cmbxSearchBy.Items.Add("Last Name");
                cmbxSearchBy.Items.Add("Academic Year");
            }
            else if (cmbxSearchTable.SelectedItem.ToString() == "Professors")
            {
                cmbxSearchBy.Items.Clear();
                cmbxSearchBy.Items.Add("Registraion Number");
                cmbxSearchBy.Items.Add("First Name");
                cmbxSearchBy.Items.Add("Last Name");
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbxSearchTable.SelectedIndex==-1)
            {
                MessageBox.Show("Invalid selection.", "Query", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cmbxSearchTable.DroppedDown = true;
            }
            else if (cmbxSearchBy.SelectedIndex == -1)
            {
                MessageBox.Show("Invalid selection.", "Query", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cmbxSearchBy.DroppedDown = true;
            }
            else if(txtValue.TextLength<1)
                MessageBox.Show("Enter a keyword to search.", "Query", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                string table="";
                string id = "";
                bool notString = true;

                if (cmbxSearchTable.SelectedIndex==0)
                {
                    table = "Module";
                    if (cmbxSearchBy.SelectedIndex == 0)
                    {
                        id = "MId";
                        notString = false;
                    }
                    else if (cmbxSearchBy.SelectedIndex == 1)
                        id = "Year";
                    else if (cmbxSearchBy.SelectedIndex == 2)
                        id = "Credits";
                }
                else if (cmbxSearchTable.SelectedIndex == 1)
                {
                    table = "Student";
                    if (cmbxSearchBy.SelectedIndex == 0)
                        id = "Reg_No";
                    else if (cmbxSearchBy.SelectedIndex == 1)
                    {
                        id = "FName";
                        notString = false;
                    }
                    else if (cmbxSearchBy.SelectedIndex == 2)
                    {
                        id = "LName";
                        notString = false;
                    }
                else if (cmbxSearchBy.SelectedIndex == 3)
                        id = "Year";
                }
                else if (cmbxSearchTable.SelectedIndex == 2)
                {
                    table = "Professor";
                    if (cmbxSearchBy.SelectedIndex == 0)
                        id = "PId";
                    else if (cmbxSearchBy.SelectedIndex == 1)
                    {
                        id = "FName";
                        notString = false;
                    }
                    else if (cmbxSearchBy.SelectedIndex == 2)
                    {
                        id = "LName";
                        notString = false;
                    }
                }

                SqlDataAdapter adt;//sandeepa
                if (txtValue.Text != "*") { 
                if(notString)
                    adt= new SqlDataAdapter("select * from "+table+" where "+id+"="+txtValue.Text, con);
                else
                    adt= new SqlDataAdapter("select * from "+table+" where "+id+"='"+txtValue.Text+"'", con);
                }
                else
                    adt = new SqlDataAdapter("select * from " + table, con);
                
                DataTable dt = new DataTable();
                try
                {
                    con.Open();
                    adt.Fill(dt);
                    con.Close();
                    datagrid.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Query", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Student 
        //Students section//
        //In this section, the admin can manage all of the student details.
        //Here include student registration, details update and more.
        private void pbxSSearch_Click(object sender, EventArgs e) //search option
        {
            try
            {
                cmd = new SqlCommand("select count(Reg_No) from Student where Reg_No=" + txtSearch.Text, con);
                con.Open();
                int recordExists = (int)cmd.ExecuteScalar();
                con.Close();
                if (recordExists > 0)
                {
                    cmd = new SqlCommand("select * from Student where Reg_No=" + txtSearch.Text, con);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        txtSReg.Text = reader["Reg_No"].ToString();
                        txtSFname.Text = reader["FName"].ToString();
                        txtSLname.Text = reader["LName"].ToString();
                        txtSAddress.Text = reader["Address"].ToString();
                        txtSEmail.Text = reader["Email"].ToString();
                        txtSPhone.Text = reader["Phone"].ToString();
                        radSM.Checked = (bool)reader["Gender"];
                        radSF.Checked = !radSM.Checked;
                        cmbxSYear.SelectedIndex = (int)reader["Year"] - 1;
                        dtpSDob.Value = DateTime.ParseExact(reader["DOB"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        digry.Text = reader["Digry"].ToString();
                        txtSFac.Text = reader["Faculty"].ToString();
                        SModule1.Text = reader["Module1"].ToString();
                        SModule2.Text = reader["Module2"].ToString();
                        SModule3.Text = reader["Module3"].ToString();
                        SModule4.Text = reader["Module4"].ToString();
                        SModule5.Text = reader["Module5"].ToString();
                    }
                }
                else
                {
                    txtSReg.Clear();
                    txtSFname.Clear();
                    txtSLname.Clear();
                    txtSAddress.Clear();
                    txtSEmail.Clear();
                    txtSPhone.Clear();
                    cmbxSYear.SelectedIndex = -1;
                    dtpSDob.Value = DateTime.Today;
                    radSM.Checked = true;
                    radSF.Checked = false;
                    digry.Clear();
                    SModule2.Clear();
                    SModule3.Clear();
                    SModule4.Clear();
                    SModule5.Clear();
                    SModule1.Clear();
                    MessageBox.Show("Search failed. Record does not exists.", "Students", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Retrieving data from Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            con.Close(); //search option


            // Erro Provider
            double i = 0;
            if (double.TryParse(txtSearch.Text,out i))
            {
                errorProvider1.SetError(txtSearch, "");
            }
            else
            {
                errorProvider1.SetError(txtSearch, "Search Option Allowed ID Number Only");
            }
            // Erro Provider end

        }
        private void btnSSave_Click(object sender, EventArgs e)
        {
            int recordExists;
            cmd = new SqlCommand("select count(PId) from Professor where PId=" + txtSReg.Text, con);
            try
            {
                if (txtSReg.TextLength < 5 || !IsDigitsOnly(txtSReg.Text))
                    MessageBox.Show("Invalid Registration Number. Registration number should be longer than 5 digits.", "Students", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtSFname.TextLength < 1)
                    MessageBox.Show("Invalid First Name.", "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtSLname.TextLength < 1)
                    MessageBox.Show("Invalid Last Name.", "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtSAddress.TextLength < 1)
                    MessageBox.Show("Invalid Address.", "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtSEmail.TextLength < 1)
                    MessageBox.Show("Invalid Email Address.", "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtSPhone.TextLength < 1 || !IsDigitsOnly(txtSPhone.Text))
                    MessageBox.Show("Invalid Phone Number.", "Students", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (cmbxSYear.SelectedIndex < 0)
                {
                    MessageBox.Show("Invalid Academic Year.", "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cmbxSYear.DroppedDown = true;
                }
                else {
                    con.Open();
                    recordExists = (int)cmd.ExecuteScalar();
                    con.Close();
                    cmd = new SqlCommand("select count(Reg_No) from Student where Reg_No=" + txtSReg.Text, con);

                    if (recordExists > 0)
                        MessageBox.Show("User with the same user Id exists.", "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    else {
                        con.Open();
                        recordExists = (int)cmd.ExecuteScalar();
                        con.Close();

                        if (recordExists < 1)
                        {
                            cmd = new SqlCommand("insert into Student values(" + txtSReg.Text + ",'" + txtSFname.Text + "','" + txtSLname.Text + "','" + radSM.Checked.ToString() + "','" + dtpSDob.Value.Date.ToString("dd-MM-yyyy") + "','" + txtSAddress.Text + "','" + txtSEmail.Text + "'," + txtSPhone.Text + "," + (cmbxSYear.SelectedIndex + 1) + ",'" + digry.Text + "','" + txtSFac.Text + "','" + SModule1.Text + "','" + SModule2.Text + "','" + SModule3.Text + "','" + SModule4.Text + "','" + SModule5.Text + "')", con);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            cmd = new SqlCommand("insert into Auth values(" + txtSReg.Text + ",'123',1,'" + txtSReg.Text + "')", con);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            MessageBox.Show("Student '" + txtSReg.Text + "' Registered.", "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            SqlCommand cmd = new SqlCommand("update Student set FName='" + txtSFname.Text + "',LName='" + txtSLname.Text + "',Gender='" + radSM.Checked.ToString() + "',DOB='" + dtpSDob.Value.Date.ToString("dd-MM-yyyy") + "',Address='" + txtSAddress.Text + "',Email='" + txtSEmail.Text + "',Phone=" + txtSPhone.Text + ",Year=" + (cmbxSYear.SelectedIndex + 1) + ",Digry='" + digry.Text + "',Faculty='" + txtSFac.Text + "',Module1='" + SModule1.Text + "',Module2='" + SModule2.Text + "',Module3='" + SModule3.Text + "',Module4='" + SModule4.Text + "',Module5='" + SModule5.Text + "' where Reg_No=" + txtSReg.Text, con);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            MessageBox.Show("Student '" + txtSReg.Text + "' Updated.", "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSDelete_Click(object sender, EventArgs e)
        {
            if (txtReset.Text != Id.ToString()) { 
            try
            {
                cmd = new SqlCommand("delete from Student where Reg_No=" + txtSReg.Text, con);
                con.Open();
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand("delete from Auth where Id=" + txtSReg.Text, con);
                cmd.ExecuteNonQuery();
                txtSFname.Clear();
                txtSLname.Clear();
                txtSAddress.Clear();
                txtSEmail.Clear();
                txtSPhone.Clear();
                cmbxSYear.SelectedIndex = -1;
                dtpSDob.Value = DateTime.Today;
                radSM.Checked = true;
                radSF.Checked = false;
                digry.Clear();
                txtSFac.Clear();
                SModule2.Clear();
                SModule3.Clear();
                SModule4.Clear();
                SModule5.Clear();
                SModule1.Clear();
                MessageBox.Show("Student '" + txtSReg.Text + "' was removed.", "Students", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSReg.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Student Registration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            con.Close();
            }
        }
        #endregion

        #region Professor
        //Professor section//
        //In this section, the admin can manage all of the professor details.
        //Here include student registration, details update and more.
        private void pbxPSearch_Click(object sender, EventArgs e)
        {
            try
            {
                cmd = new SqlCommand("select count(PId) from Professor where PId=" + txtPsearch.Text, con);
                con.Open();
                int recordExists = (int)cmd.ExecuteScalar();
                con.Close();
                if (recordExists > 0)
                {
                    cmd = new SqlCommand("select * from Professor where PId=" + txtPsearch.Text, con);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        txtPReg.Text = reader["PID"].ToString();
                        txtPFname.Text = reader["FName"].ToString();
                        txtPLname.Text = reader["LName"].ToString();
                        txtPAddress.Text = reader["Address"].ToString();
                        txtPEmail.Text = reader["Email"].ToString();
                        txtPPhone.Text = reader["Phone"].ToString();
                        txtPQf.Text = reader["Qualifications"].ToString();
                        txtPUsername.Text = reader["Username"].ToString();
                        radPM.Checked = (bool)reader["Gender"];
                        radPF.Checked = !radSM.Checked;
                        dtpPDob.Value = DateTime.ParseExact(reader["DOB"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    txtPReg.Clear();
                    txtPFname.Clear();
                    txtPLname.Clear();
                    txtPAddress.Clear();
                    txtPEmail.Clear();
                    txtPPhone.Clear();
                    txtPUsername.Clear();
                    txtPQf.Clear();
                    dtpPDob.Value = DateTime.Today;
                    radPM.Checked = true;
                    radPF.Checked = false;
                    modulelist.Clear();
                    MessageBox.Show("Search failed. Record does not exists.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Retrieving data from Database - Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            con.Close();
        }

        private void btnPSave_Click(object sender, EventArgs e)
        {
            cmd = new SqlCommand("select count(Reg_No) from Student where Reg_No=" + txtPReg.Text, con);
            SqlCommand cmd2 = new SqlCommand("select count(PId) from Professor where Username='" + txtPUsername.Text+"' And Not PId="+txtPReg.Text, con);
            int recordExists=0, recordExists2 = 0;
            try
            {
                if (txtPReg.TextLength < 5 || !IsDigitsOnly(txtPReg.Text))
                    MessageBox.Show("Invalid Registration Number. Registration number should be longer than 5 digits.", "Professors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtPUsername.TextLength < 1 || txtPUsername.Text.Contains(" "))
                    MessageBox.Show("Invalid Username.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtPFname.TextLength < 1)
                    MessageBox.Show("Invalid First Name.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtPLname.TextLength < 1)
                    MessageBox.Show("Invalid Last Name.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtPAddress.TextLength < 1)
                    MessageBox.Show("Invalid Address.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtPEmail.TextLength < 1)
                    MessageBox.Show("Invalid Email Address.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtPPhone.TextLength < 1 || !IsDigitsOnly(txtSPhone.Text))
                    MessageBox.Show("Invalid Phone Number.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else if (txtPQf.TextLength < 1)
                    MessageBox.Show("Invalid Qualifications.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else {
                    con.Open();
                    recordExists2 = (int)cmd2.ExecuteScalar();
                    recordExists = (int)cmd.ExecuteScalar();
                    con.Close();
                    cmd = new SqlCommand("select count(PId) from Professor where PId=" + txtPReg.Text, con);
                    if (recordExists > 0)
                    {
                        MessageBox.Show("User with the same user Id exists.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    if (recordExists2 > 0)
                    { 
                        MessageBox.Show("Username exists.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    con.Open();
                    recordExists = (int)cmd.ExecuteScalar();
                    con.Close();

                    if (recordExists < 1)
                    {
                        cmd = new SqlCommand("insert into Professor values(" + txtPReg.Text + ",'" + txtPFname.Text + "','" + txtPLname.Text + "','" + radPM.Checked.ToString() + "','" + dtpPDob.Value.Date.ToString("dd-MM-yyyy") + "','" + txtPAddress.Text + "','" + txtPEmail.Text + "'," + txtPPhone.Text + ",'" + txtPQf.Text + "','" + txtPUsername.Text + "')", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        cmd = new SqlCommand("insert into Auth values(" + txtPReg.Text + ",'123',2,'" + txtPUsername.Text + "')", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        
                        if (modulelist[0] != "*") { 
                            cmd = new SqlCommand("delete from Module_Prof where PId=" + txtPReg.Text, con);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            foreach (string x in modulelist)
                            {
                                cmd = new SqlCommand("insert into Module_Prof values(" + txtPReg.Text + ",'" + x + "')", con);
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                        MessageBox.Show("Professor '" + txtPReg.Text + "' Registered.", "Professors", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("update Professor set FName='" + txtPFname.Text + "',LName='" + txtPLname.Text + "',Gender='" + radPM.Checked.ToString() + "',DOB='" + dtpPDob.Value.Date.ToString("dd-MM-yyyy") + "',Address='" + txtPAddress.Text + "',Email='" + txtPEmail.Text + "',Phone=" + txtPPhone.Text + ",Qualifications='" + txtPQf.Text+ "',Username='" + txtPUsername.Text + "' where PId=" + txtPReg.Text, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        cmd = new SqlCommand("delete from Auth where Id=" + txtPReg.Text, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        cmd = new SqlCommand("insert into Auth values(" + txtPReg.Text + ",'123',2,'" + txtPUsername.Text + "')", con);//professor password generate here. 
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        if (modulelist[0] != "*")
                        {
                            cmd = new SqlCommand("delete from Module_Prof where PId=" + txtPReg.Text, con);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            foreach (string x in modulelist)
                            {
                                cmd = new SqlCommand("insert into Module_Prof values(" + txtPReg.Text + ",'" + x + "')", con);
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                        MessageBox.Show("Professor '" + txtPReg.Text + "'record update succesfull.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPDelete_Click(object sender, EventArgs e)
        {
            if (txtReset.Text != Id.ToString())
            {
                try
                {
                    cmd = new SqlCommand("delete from Module_Prof where PId=" + txtPReg.Text, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand("delete from Professor where PId=" + txtPReg.Text, con);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand("delete from Auth where Id=" + txtPReg.Text, con);
                    cmd.ExecuteNonQuery();
                    txtPFname.Clear();
                    txtPLname.Clear();
                    txtPAddress.Clear();
                    txtPEmail.Clear();
                    txtPPhone.Clear();
                    txtPQf.Clear();
                    txtPUsername.Clear();
                    dtpPDob.Value = DateTime.Today;
                    radPM.Checked = true;
                    radPF.Checked = false;
                    MessageBox.Show("Professor '" + txtSReg.Text + "' was removed.", "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPReg.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Professor Registration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                con.Close();
            }
        }
     
        private void btnPModules_Click(object sender, EventArgs e)
        {
            Modules modules = new Modules(txtPReg.Text);
            modulelist.Clear();
            modulelist.Add("*");
            var result = modules.ShowDialog();
            if (result == DialogResult.OK)
                modulelist = modules.modulelist;
        }
        #endregion

        private void Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MaterialTabSelector1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
           
        }

        private void txtSReg_Click(object sender, EventArgs e)
        {
            autogen();
        }

        private void txtPReg_Click(object sender, EventArgs e)
        {
            autogen();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
        }

        private void materialFlatButton2_Click(object sender, EventArgs e)
        {
            if (txtReset.Text != Id.ToString())
            {
                try
                {                                       // Admin Password detail update heare ....
                    SqlCommand cmd = new SqlCommand("update Auth set Password='123' where Username='" + txtReset.Text + "'", con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User ID: '" + txtReset.Text + "', user's password was changed to default password '123'", "Password Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Password Reset", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                con.Close();
            }
        }

        private void btnRese_Click(object sender, EventArgs e)
        {

        }

        private void txtSearch_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void datagrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void materialLabel28_Click(object sender, EventArgs e)
        {

        }

        private void cmbxSYear_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbxSFac_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}