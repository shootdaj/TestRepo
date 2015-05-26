git init
git remote add configrepo http://github.com/shootdaj/ZoneLightingTestConfig
git fetch configrepo
git checkout -b master --track configrepo/master