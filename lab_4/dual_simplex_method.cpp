#include <iostream>
#include <fstream>
#include <vector>
#include <sstream>
#include <algorithm>
#include <limits>

using namespace std;

class SimplexSolver 
{
private:
    string problem_type;

    vector<double> c;
    vector<vector<double>> a;
    vector<double> b;

    int limits;
    int variables;

    vector<int> basis;
    vector<double> c_basis;

    vector<vector<double>> table;    
    vector<double> deltas;

    void init() 
    {
        this->limits = b.size();
        this->variables = c.size();

        this->basis = vector<int>(limits, 0);
        this->c_basis = vector<double>(limits, 0);

        this->table = vector<vector<double>>(limits, vector<double>(variables + 1, 0));    
        this->deltas = vector<double>(variables + 1, 0);


        c.push_back(0);
        for (int j = 0; j <= variables; j++) deltas[j] = -1 * c[j];

        for (int i = 0; i < limits; i++) 
        {
            basis[i] = limits + i - 1;
            c_basis[i] = c[basis[i]];
        }
        
        for (int j = 0; j <= variables; j++) 
        {
            for (int i = 0; i < limits; i++) 
            {
                table[i][j] = (j == variables) ? b[i] : a[i][j];
                
                deltas[j] += c_basis[i] * table[i][j];
            }
        } 
    }

    void check_optimality(vector<pair<int, double>> &negative_plans, vector<pair<int, double>> &negative_deltas)
    {
        for (int i = 0; i < limits; i++)
        {
            if (table[i][variables] < 0) 
                negative_plans.push_back({i, table[i][variables]});
        }

        for (int j = 0; j < variables; j++) 
        {
            if (deltas[j] < 0) 
                negative_deltas.push_back({j, deltas[j]});
        }
    }

    int check_alternative_existance() 
    {
        int alternative = -1;
        for (int j = 0; j < variables; j++) 
        {
            if (deltas[j] == 0) 
            {
                bool is_in_basis = false;
                for (int i = 0; i < limits; i++) 
                    if (j == basis[i]) is_in_basis = true;

                if (!is_in_basis) alternative = j;
            }
        }

        return alternative;
    }

    void print_two_plans(int alternative) 
    {
        if (problem_type == "max") 
            cout << "Максимальне значення функції = " << deltas[variables];
        else 
            cout << "Мінімальне значення функції = " << -1*deltas[variables];
        
        cout << "\nі може бути досягнуте у будь-якій точці відрізку, \nз кінцями в " 
        <<"\nX1 = [";

        vector<double> x1(variables, 0);

        for (int i = 0; i < limits; i++) x1[basis[i]] = table[i][variables];

        for (int j = 0; j < variables - 1; j++) cout << x1[j] << ", ";

        cout << x1[variables - 1] << "]";
    

        //one more classic simplex iter to find alternative optimal plan
        vector<pair<int, double>> has_alternative(1, {alternative, 0});
        auto [remove, add] = classic_pivot(has_alternative);
        table_rebuild(remove, add);


        cout << "\nX2 = [";

        vector<double> optimal_x(variables, 0);

        for (int i = 0; i < limits; i++) optimal_x[basis[i]] = table[i][variables];

        for (int j = 0; j < variables - 1; j++) cout << optimal_x[j] << ", ";

        cout << optimal_x[variables - 1] << "]\n";
    }

    void print_optimal_plan()
    {
        if (problem_type == "max") 
            cout << "Максимальне значення функції = " << deltas[variables];
        else 
            cout << "Мінімальне значення функції = " << -1*deltas[variables];
        
        cout << " і може бути досягнуте при X = [";

        vector<double> x2(variables, 0);

        for (int i = 0; i < limits; i++) x2[basis[i]] = table[i][variables];
        
        for (int j = 0; j < variables - 1; j++) cout << x2[j] << ", ";
        
        cout << x2[variables - 1] << "]\n";
    }

    void table_rebuild(int remove_var, int add_var) 
    {
        double pivot = table[remove_var][add_var];
        basis[remove_var] = add_var;
        c_basis[remove_var] = c[basis[remove_var]];
            
        for (int i = 0; i < limits; i++) {
            if (i != remove_var) {
                for (int j = 0; j <= variables; j++) {
                    if (j != add_var) {
                        table[i][j] -= table[i][add_var] / table[remove_var][add_var] * table[remove_var][j];
                    }
                }
            }
        }
                                
        for (int j = 0; j <= variables; j++) table[remove_var][j] /= pivot;
                    
        for (int i = 0; i < limits; i++) 
            if (i != remove_var) table[i][add_var] = 0;
                    
        for (int j = 0; j <= variables; j++) deltas[j] = -1 * c[j];
            
        for (int j = 0; j <= variables; j++) 
            for (int i = 0; i < limits; i++) 
                deltas[j] += c_basis[i] * table[i][j];
    }

    pair<int, int> classic_pivot(vector<pair<int, double>> &negative_deltas) 
    {
        //sort the negative deltas by the least optimacy
        sort(negative_deltas.begin(), negative_deltas.end(),
            [](pair<int, double>& a, pair<int, double>& b) { return a.second < b.second; }
        );
        
        int remove_var = -1;
        double remove_rating = 1e7;
        int add_var = -1;

        int to_add = negative_deltas[0].first;
        //find the basis we should replace
        for (int i = 0; i < limits; i++)  {
            //if elem is positive check if its rating is the smallest
            if (table[i][to_add] >= 0) {
                double temp = table[i][variables] / table[i][to_add];
                if (temp < remove_rating) {
                    remove_var = i;
                    remove_rating = temp;
                    add_var = to_add;
                }
            }
        }

        return {remove_var, add_var};
    }

    pair<int, int> dual_pivot(vector<pair<int, double>> &negative_plans) 
    {
        //sort the negative plans by the least optimacy
        sort(negative_plans.begin(), negative_plans.end(),
            [](pair<int, double>& a, pair<int, double>& b) { return a.second < b.second; }
        );
        
        int remove_var = -1;
        double add_rating = 1e7;
        int add_var = -1;

        int to_remove = negative_plans[0].first;
        //find the basis we should insert
        for (int j = 0; j < variables; j++)  {
            //if elem is positive check if its rating is the smallest
            if (table[to_remove][j] < 0) {
                double temp = abs(deltas[j] / table[to_remove][j]);
                if (temp < add_rating) {
                    remove_var = to_remove;
                    add_rating = temp;
                    add_var = j;
                }
            }
        }

        return {remove_var, add_var};
    }

public:
    SimplexSolver(vector<double>& c, vector<vector<double>>& a, vector<double>& b) 
    : c(c), a(a), b(b)
    {
        init();
    }

    SimplexSolver(const string& filename)
    {
        ifstream file(filename);
        if (!file) {
            cerr << "Error reading file!" << endl;
            exit(1);
        }
    
        string line;
        getline(file, line);
        stringstream stream(line);

        stream >> problem_type;

        double val;
        while (stream >> val) 
            c.push_back((problem_type == "max") ? val : -1 * val);
    
        uint size = c.size();
    
        while (getline(file, line)) {
            stringstream s(line);
            vector<double> row(size, 0);
            for (int i = 0; i < size; i++) s >> row[i];
            a.push_back(row);
            s >> val;
            b.push_back(val);
        }

        init();
    }

    void solve() 
    {
        while (true) {            
            vector<pair<int, double>> negative_deltas;
            vector<pair<int, double>> negative_plans;
            check_optimality(negative_plans, negative_deltas);

            bool is_optimal = (negative_plans.size() || negative_deltas.size()) ? false : true;
            bool is_forbidden = (negative_plans.size()) ? true : false;


            if (is_optimal)
            {
                int alternative = check_alternative_existance();

                if (alternative > -1) print_two_plans(alternative);
                else print_optimal_plan();

                return;
            }


            int remove_var = -1;
            int add_var = -1;
            if (is_forbidden) 
            {
                tie(remove_var, add_var) = dual_pivot(negative_plans);
            } 
            else if (!is_optimal && !is_forbidden) 
            {
                tie(remove_var, add_var) = classic_pivot(negative_deltas);
            } 
            else 
            {
                cout << "Під час обчислення виникла помилка!";
                return;
            }


            if (remove_var == -1) 
            {
                cout << "Цільова функція є необмеженою й оптимальних розв'язків не існує.\n";
                return;
            }

            table_rebuild(remove_var, add_var);

            cout << "Таблицю перебудовано\n";
        }
    }
};

int main() 
{
    SimplexSolver LLP("lin_prog_prob.txt");

    LLP.solve();

    return 0;
}