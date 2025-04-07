TranProblemSolver solver = new TranProblemSolver(
    "/home/dell_3420/GitHub/MMOR/lab_5/transportation_problem.txt"
);

var plan = solver.NorthWestCorner();

plan = solver.PotentialMethod(plan);

solver.SavePlan(
    "/home/dell_3420/GitHub/MMOR/lab_5/answer.txt",
    plan
);