using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBlibrary;

namespace WPFDataDML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EmpDataStore empData;
        public MainWindow()
        {
            InitializeComponent();
            empData = new EmpDataStore(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
        }

        private void ReloadEmpDataGrid()
        {
            List<Employee> empList = empData.GetAllEmps();
            DataGridEmp.ItemsSource = null;
            DataGridEmp.ItemsSource = empList;
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadEmpDataGrid();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmpNoTxt.Text == "")
                {
                    MessageBox.Show("Enter Empno");
                    return;
                }
                Employee emp = new Employee()
                {
                    EmpName = EmpNameTxt.Text,
                    Empno = Convert.ToInt32(EmpNoTxt.Text),
                    Hiredate = Convert.ToDateTime(HireDateTxt.Text),
                    Salary = Convert.ToDecimal(SalaryTxt.Text)
                };

                if (empData.InsertEmpDetails(emp))
                {
                    MessageBox.Show("Successfully inserted");
                    ReloadEmpDataGrid();
                }
                else
                {
                    MessageBox.Show("Not inserted");
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (FormatException fex)
            {
                MessageBox.Show(fex.Message);
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmpNoTxt.Text == "")
                {
                    MessageBox.Show("Enter Empno");
                    return;
                }
                Employee emp = new Employee()
                {
                    EmpName = EmpNameTxt.Text,
                    Empno = Convert.ToInt32(EmpNoTxt.Text),
                    Hiredate = Convert.ToDateTime(HireDateTxt.Text),
                    Salary = Convert.ToDecimal(SalaryTxt.Text)
                };
                if (empData.UpdateEmpByNo(emp))
                {
                    MessageBox.Show("Successfully Updated");
                    ReloadEmpDataGrid();
                }
                else
                {
                    MessageBox.Show("Not Updated");
                }
            }
            catch (FormatException fex)
            {
                MessageBox.Show(fex.Message);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you wand to delete Entry ?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    if (EmpNoTxt.Text == "")
                    {
                        MessageBox.Show("Enter Empno To delete");
                        return;
                    }
                    int empno = Convert.ToInt32(EmpNoTxt.Text);
                    if (empData.DeleteEmpByNo(empno))
                    {
                        MessageBox.Show("Successfully Deleted");
                        ReloadEmpDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("Not Deleted");
                    }
                }
                catch(FormatException fex)
                {
                    MessageBox.Show(fex.Message);
                }
                catch(SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            EmpNameTxt.Text = "";
            EmpNoTxt.Text = "";
            HireDateTxt.Text = "";
            SalaryTxt.Text = "";
            DataGridEmp.ItemsSource = null;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmpNoTxt.Text == "")
                {
                    MessageBox.Show("Please Enter Empno To Search");
                    return;
                }
                List<Employee> empList = new List<Employee>() { empData.GetEmployeeByNo(Convert.ToInt32(EmpNoTxt.Text)) };
                DataGridEmp.ItemsSource = null;
                DataGridEmp.ItemsSource = empList;
            }
            catch(FormatException fex)
            {
                MessageBox.Show(fex.Message);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
