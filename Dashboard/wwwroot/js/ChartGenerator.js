var ChartEnum = {
    1: "DayChartButton",
    2: "WeekChartButton",
    3: "MonthChartButton"
}
var dataMass = [];
var ChartView;
var myUrl = $(".ChartConteiner").data("RetriveUrl");

$(document).ready(() => {

    //let ActiveChart = ChartEnum.1;

    const ctx = document.getElementById('MyChart');

    var userId = parseInt($(".ChartBox").data("userid"), 10);

    console.log("I live");
    let i = 0;
    while (i  < 365)
    {
        let y = 0;
        while (y < 3)
        {
            let Data = new Date();
            Data.setDate(Data.getDate() + i);
            dataMass.push({ timeOfTest: Data.toISOString(), vibeLevel: Math.random() * (5 - 1) + 1 });
            y++;
        }
        i++;
    }

    //console.log(dataMass);

    //$.ajax({
    //    type: 'GET',
    //    url: myUrl,
    //    cache: 'false',
    //    contentType: 'application/json',
    //    data: { userId },
    //    success: function (VibeData) {
    //        dataMass = VibeData
    //    },
    //    error: {}
    //}); 

    GenereteChartByDay(dataMass, ctx);

    $("#DayChartButton").on("click", () => {
        GenereteChartByDay(dataMass, ctx)
    })
    $("#WeekChartButton").on("click", () => {
        GenereteChartByWeek(dataMass, ctx)
    })
    $("#MonthChartButton").on("click", () => {
        GenereteChartByMonth(dataMass, ctx)
    })
    


});

function GenereteChartByDay(dataArr, ctx) {
    let ChartData = [];
    let ChartLabels = [];
    const DataMapDictonary = new Map();

    if (ChartView)
    {
        ChartView.destroy();
    }


    dataArr.forEach((element) =>
    {
        let TimeSplit = element.timeOfTest.substr(0, element.timeOfTest.indexOf("T"));
        if (DataMapDictonary.has(TimeSplit)) {
            let objByKey = DataMapDictonary.get(TimeSplit);

            objByKey.vibeScore = objByKey.vibeScore + element.vibeLevel;
            objByKey.AskCount = objByKey.AskCount + 1;
            DataMapDictonary.set(TimeSplit, objByKey);
        }
        else {
            DataMapDictonary.set(TimeSplit, { vibeScore: element.vibeLevel, AskCount: 1 });
        }
    });

    DataMapDictonary.forEach((value, key, map) =>
    {

        ChartLabels.push(key);
        let val = value.vibeScore / value.AskCount;


        ChartData.push(val);

    })

    console.log(ChartLabels);
    console.log(ChartData);


    ChartView = new Chart(ctx, {
        type: "line",
        data: {
            labels:  ChartLabels,
            datasets: [{
                label: 'Level of Vibes by Dates',
                data: ChartData,
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

function GenereteChartByWeek(dataArr, ctx) {
    let ChartData = [];
    let ChartLabels = [];
    const DataMapDictonary = new Map();
    if (ChartView) {
        ChartView.destroy();
    }

   


    dataArr.forEach((element) => {
        let DateOfStr = new Date(element.timeOfTest);

        let StartOfWeek = new Date(element.timeOfTest);
        let EndOfWeek = new Date(element.timeOfTest);
        StartOfWeek.setDate(DateOfStr.getDate() - DateOfStr.getDay());
        EndOfWeek.setDate(DateOfStr.getDate() + (6 - DateOfStr.getDay()));

        let TimeSplit = (StartOfWeek.toISOString().substr(0, StartOfWeek.toISOString().indexOf("T"))) + " - " + (EndOfWeek.toISOString().substr(0, EndOfWeek.toISOString().indexOf("T")));



        if (DataMapDictonary.has(TimeSplit)) {
            let objByKet = DataMapDictonary.get(TimeSplit);

            objByKet.vibeScore = objByKet.vibeScore + element.vibeLevel;
            objByKet.AskCount = objByKet.AskCount + 1;
            DataMapDictonary.set(TimeSplit, objByKet);
        }
        else {
            DataMapDictonary.set(TimeSplit, { vibeScore: element.vibeLevel, AskCount: 1 });
            console.log(TimeSplit);

        }
    });

    DataMapDictonary.forEach((value, key, map) => {
        ChartLabels.push(key);
        ChartData.push(value.vibeScore / value.AskCount);

    })

    ChartView = new Chart(ctx, {
        type: "line",
        data: {
            labels: ChartLabels,
            datasets: [{
                label: 'Level of Vibes by Dates',
                data: ChartData,
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        },

    });
}

function GenereteChartByMonth(dataArr, ctx) {
    let ChartData = [];
    let ChartLabels = [];
    const DataMapDictonary = new Map();
    if (ChartView) {
        ChartView.destroy();
    }

    dataArr.forEach((element) => {
        let DateOfStr = new Date(element.timeOfTest);
        let Year = DateOfStr.getFullYear();
        let Month = DateOfStr.getMonth() + 1;
        let TimeSplit = Month < 10 ? `0${Month}/${Year}` : `${Month}/${Year}`;


        if (DataMapDictonary.has(TimeSplit)) {
            let objByKet = DataMapDictonary.get(TimeSplit);

            objByKet.vibeScore = (objByKet.vibeScore + element.vibeLevel);

            objByKet.AskCount = objByKet.AskCount + 1;
            DataMapDictonary.set(TimeSplit, objByKet);
            console.log(DataMapDictonary.get(TimeSplit));

        }
        else {
            DataMapDictonary.set(TimeSplit, { vibeScore: element.vibeLevel, AskCount: 1 });
            console.log(TimeSplit);

        }
    });

    DataMapDictonary.forEach((value, key, map) => {
        ChartLabels.push(key);
        ChartData.push(value.vibeScore / value.AskCount);
        console.log(value.vibeScore);
        console.log(value.AskCount);


    })

    ChartView = new Chart(ctx, {
        type: "line",
        data: {
            labels: ChartLabels,
            datasets: [{
                label: 'Level of Vibes by Dates',
                data: ChartData,
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}



