#include <iostream>
#include <fstream>
#include <vector>
#include <sstream>
#include <algorithm>
#include <limits>

using namespace std;

void read_lpp(const string& filename, vector<double>& c, vector<vector<double>>& a, vector<double>& b) {
    ifstream file(filename);
    if (!file) {
        cerr << "Error reading file!" << endl;
        exit(1);
    }

    string line;
    getline(file, line);
    stringstream stream(line);

    double val;
    while (stream >> val) c.push_back(val);

    uint size = c.size();

    while (getline(file, line)) {
        stringstream s(line);
        vector<double> row(size, 0);
        for (int i = 0; i < size; i++) s >> row[i];
        a.push_back(row);
        s >> val;
        b.push_back(val);
    }
}

void solve_simplex(vector<double>& c, vector<vector<double>>& a, vector<double>& b) {
    int limits = b.size();
    int variables = c.size();

    vector<int> basis(limits, 0);
    vector<double> c_i(limits, 0);

    vector<vector<double>> table(limits, vector<double>(variables + 1, 0));    
    vector<double> deltas(variables + 1, 0);

    //initialization
    c.push_back(0);
    for (int j = 0; j <= variables; j++) deltas[j] = -1 * c[j];

    for (int i = 0; i < limits; i++) {
        //defining basis vars
        basis[i] = limits + i - 1;

        //fill coeficients of the basis vars
        c_i[i] = c[basis[i]];
    }
    for (int j = 0; j <= variables; j++) {
        for (int i = 0; i < limits; i++) {
            //fill the table and plan
            table[i][j] = (j == variables) ? b[i] : a[i][j];
            
            deltas[j] += c_i[i] * table[i][j];
        }
    } 


    //iteration
    while (true) {
        bool is_optimal = true;
        int has_alternative = -1;

        //find unoptimal deltas
        vector<pair<int, double>> negative_deltas;
        for (int j = 0; j < variables; j++) {
            if (deltas[j] < 0) {
                negative_deltas.push_back({j, deltas[j]});
                is_optimal = false;
            }

            //if plan is optimal check if there is alternative
            if (is_optimal && deltas[j] == 0) {
                bool is_in_basis = false;
                for (int i = 0; i < limits; i++) 
                    if (j == basis[i]) is_in_basis = true;

                if (!is_in_basis) has_alternative = j;
            }
        }

        //find second end of the optimal line
        if (is_optimal && has_alternative > -1) {
            cout << "Максимальне значення функції = " << deltas[variables];
            cout << "\nі може бути досягнуте у будь-якій точці відрізку, \nз кінцями в " 
            <<"\nX1 = [";

            vector<double> x1(variables, 0);

            for (int i = 0; i < limits; i++) x1[basis[i]] = table[i][variables];

            for (int j = 0; j < variables - 1; j++) cout << x1[j] << ", ";

            cout << x1[variables - 1] << "]";
    
        
            int remove_var = -1;
            double remove_rating = 1e7;
    
            //find the basis we should remove
            for (int i = 0; i < limits; i++)  {
                //if elem is positive check if ist rating is the smallest
                if (table[i][has_alternative] >= 0) {
                    double temp = table[i][variables] / table[i][has_alternative];
                    if (temp < remove_rating) {
                        remove_var = i;
                        remove_rating = temp;
                    }
                }
            }
    

            double pivot = table[remove_var][has_alternative];
            basis[remove_var] = has_alternative;
            c_i[remove_var] = c[basis[remove_var]];
    
            for (int i = 0; i < limits; i++) {
                if (i != remove_var) {
                    for (int j = 0; j <= variables; j++) {
                        if (j != has_alternative) {
                            table[i][j] -= table[i][has_alternative] / table[remove_var][has_alternative] * table[remove_var][j];
                        }
                    }
                }
            }
                        
            for (int j = 0; j <= variables; j++) {
                table[remove_var][j] /= pivot;
            }
    
            for (int i = 0; i < limits; i++) {
                if (i != remove_var) table[i][has_alternative] = 0;
            }
    
            for (int j = 0; j <= variables; j++) deltas[j] = -1 * c[j];
    
            for (int j = 0; j <= variables; j++) {
                for (int i = 0; i < limits; i++) {
                    deltas[j] += c_i[i] * table[i][j];
                }
            } 
    
            cout << "\nX2 = [";

            vector<double> optimal_x(variables, 0);

            for (int i = 0; i < limits; i++) optimal_x[basis[i]] = table[i][variables];

            for (int j = 0; j < variables - 1; j++) cout << optimal_x[j] << ", ";

            cout << optimal_x[variables - 1] << "]\n";
            return;
        }

        //print the single answer
        if (is_optimal && has_alternative == -1) {
            cout << "Максимальне значення функції = " << deltas[variables] 
            << " і може бути досягнуте при X = [";

            vector<double> x2(variables, 0);

            for (int i = 0; i < limits; i++) x2[basis[i]] = table[i][variables];
            
            for (int j = 0; j < variables - 1; j++) cout << x2[j] << ", ";
            
            cout << x2[variables - 1] << "]\n";
            return;
        }


        //sort the negative deltas by the least optimacy
        sort(negative_deltas.begin(), negative_deltas.end(),
          [](pair<int, double>& a, pair<int, double>& b) { return a.second < b.second; }
        );

        
        int remove_var = -1;
        double remove_rating = 1e7;
        int add_var = -1;

        pair<int, double> to_add = negative_deltas[0];
        //find the basis we should replace
        for (int i = 0; i < limits; i++)  {

            //if elem is positive check if ist rating is the smallest
            if (table[i][to_add.first] >= 0) {
                double temp = table[i][variables] / table[i][to_add.first];
                if (temp < remove_rating) {
                    remove_var = i;
                    remove_rating = temp;
                    add_var = to_add.first;
                }
            }
        }

        if (remove_var == -1) {
            cout << "Цільова функція є необмеженою й оптимальних розв'язків не існує.\n";
            return;
        }

        //rebuilding of the simplex table
        double pivot = table[remove_var][add_var];
        basis[remove_var] = add_var;
        c_i[remove_var] = c[basis[remove_var]];

        for (int i = 0; i < limits; i++) {
            if (i != remove_var) {
                for (int j = 0; j <= variables; j++) {
                    if (j != add_var) {
                        table[i][j] -= table[i][add_var] / table[remove_var][add_var] * table[remove_var][j];
                    }
                }
            }
        }
                    
        for (int j = 0; j <= variables; j++) {
            table[remove_var][j] /= pivot;
        }

        for (int i = 0; i < limits; i++) {
            if (i != remove_var) table[i][add_var] = 0;
        }


        for (int j = 0; j <= variables; j++) deltas[j] = -1 * c[j];

        for (int j = 0; j <= variables; j++) {
            for (int i = 0; i < limits; i++) {
                deltas[j] += c_i[i] * table[i][j];
            }
        } 

        cout << "Таблицю перебудовано\n";
    }
}

int main() {
    vector<double> c, b;
    vector<vector<double>> a;

    read_lpp("lin_prog_prob.txt", c, a, b);

    solve_simplex(c, a, b);

    return 0;
}