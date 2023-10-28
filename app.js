const express = require("express")
const app = express()

let users = [];

app.use(express.json());

app.get('/', (req, res) => res.send('Server Running!'));

app.post('/scores/postscore', (req, res) => {
    const {id, score} = req.body;

    let result = {
        cmd: -1,
        message: ''
    }

    let user = users.find((x) => x.id == id);

    if(user == undefined)
    {
        users.push({id, score});

        result.cmd = 1001;
        result.message = '신규 등록이 완료되었습니다.'
    }
    else
    {
        if(score > user.score)
        {
            user.score = score;

            result.cmd = 1002;
            result.message = '최고기록이 갱신되었습니다.'
        }
        else
        {
            result.cmd = 1003;
        }
    }

    console.log(user);
    res.send(result);
})

app.get('/scores/top3', (req, res)=>
{
    let result = users.sort(function(a, b){
        return b.score - a.score;
    })

    result = result.slice(0, 3);

    res.send({
        cmd: 1101,
        message: '',
        result
    })
})

app.get('/scores/findscore/:id', (req, res) => {
    console.log('id: ' + req.params.id)

    let user = users.find((x) => x.id == req.params.id);

    if(user == undefined)
    {
        res.send({
            cmd: 2001,
            message: '유저를 찾을 수 없습니다.'
        })
    }
    else
    {
        res.send({
            cmd: 1102,
            message: '',
            result: user
        })
    }
})

app.listen(3030, ()=>{
    console.log('server is running at 3030 port.');
});