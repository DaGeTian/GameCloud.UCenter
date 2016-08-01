var common = {
    getNArray: function (n) {
        return Array.apply(null, { length: n }).map(Number.call, Number);
    },
    getNRandomArray: function (n) {
        return Array.apply(null, { length: n }).map(Function.call, Math.random);
    }
};
