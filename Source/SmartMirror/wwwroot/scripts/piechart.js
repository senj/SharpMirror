function renderPieChart(name, labels, datasets) {
    var ctx = document.getElementById(name).getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: datasets
        }
    });
}