import copy

projects = [
    [(1, 0.3), (4, 0.8), (1, 0.4), (2, 0.6), (3, 0.8)],
    [(1, 0.3), (4, 0.8), (4, 1.4), (2, 0.6), (4, 1.0)],
    [(2, 0.5), (2, 0.4), (4, 1.4), (5, 1.5), (4, 1.0)],
]

projects = [[(price, price * (multiplier+1)) for price, multiplier in proj] for proj in projects]

steps = len(projects)
money = 6

dp_len = money + 1

dp_0 = [-100000 for _ in range(dp_len)]
dp_0[0] = 0
plans_0 = [[] for _ in range(dp_len)]

dp = copy.deepcopy(dp_0) 
plans = copy.deepcopy(plans_0)

for company in range(steps-1, -1, -1):
    for var, (price, profit) in enumerate(projects[company]):
        for m in range(price, dp_len):
            possible = profit + dp_0[m - price]
            if possible > dp[m]:
                dp[m] = possible
                plans[m] = plans_0[m - price] + [(company + 1, var+1, price, profit)]

    dp_0 = copy.deepcopy(dp) 
    plans_0 = copy.deepcopy(plans)

print(f"\nMax gain: {dp_0[money]:.2f}, Investments: {list(reversed(plans_0[money]))}\n")