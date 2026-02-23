$git = "C:\Program Files\Git\cmd\git.exe"
& $git init
& $git config user.name "ZenDesk Developer"
& $git config user.email "zendesk@example.com"
& $git add .
& $git commit -m "feat: 初版全量交付 (ZenDesk)"
& $git branch -M main
& $git remote add origin https://github.com/youfei0719/ZenDesk.git
& $git push -u origin main
