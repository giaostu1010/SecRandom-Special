with open(r'D:\GitHub\SecRandom\SecRandom\Langs\SettingsPages\ListManagementPage\Resources.resx', 'r', encoding='utf-8') as f:
    content = f.read()

# 打印最后几行
lines = content.split('\n')
for i, line in enumerate(lines[-10:], start=len(lines)-9):
    print(f'{i}: {repr(line)}')
