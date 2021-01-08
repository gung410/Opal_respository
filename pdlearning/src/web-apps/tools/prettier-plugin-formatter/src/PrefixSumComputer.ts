export class PrefixSumIndexOfResult {
  public _prefixSumIndexOfResultBrand: void;

  public index: number;
  public remainder: number;

  constructor(index: number, remainder: number) {
    this.index = index;
    this.remainder = remainder;
  }
}

export class PrefixSumComputer {
  /**
   * values[i] is the value at index i
   */
  private values: Uint32Array;

  /**
   * prefixSum[i] = SUM(heights[j]), 0 <= j <= i
   */
  private prefixSum: Uint32Array;

  /**
   * prefixSum[i], 0 <= i <= prefixSumValidIndex can be trusted
   */
  private readonly prefixSumValidIndex: Int32Array;

  constructor(values: Uint32Array) {
    this.values = values;
    this.prefixSum = new Uint32Array(values.length);
    this.prefixSumValidIndex = new Int32Array(1);
    this.prefixSumValidIndex[0] = -1;
  }

  public getTotalValue(): number {
    if (this.values.length === 0) {
      return 0;
    }
    return this._getAccumulatedValue(this.values.length - 1);
  }

  public getIndexOf(accumulatedValue: number): PrefixSumIndexOfResult {
    accumulatedValue = Math.floor(accumulatedValue); // @perf

    // Compute all sums (to get a fully valid prefixSum)
    this.getTotalValue();

    let low: number = 0;
    let high: number = this.values.length - 1;
    let mid: number = 0;
    let midStop: number = 0;
    let midStart: number = 0;

    while (low <= high) {
      // tslint:disable-next-line:no-bitwise
      mid = (low + (high - low) / 2) | 0;

      midStop = this.prefixSum[mid];
      midStart = midStop - this.values[mid];

      if (accumulatedValue < midStart) {
        high = mid - 1;
      } else if (accumulatedValue >= midStop) {
        low = mid + 1;
      } else {
        break;
      }
    }

    return new PrefixSumIndexOfResult(mid, accumulatedValue - midStart);
  }
  public getAccumulatedValue(index: number): number {
    if (index < 0) {
      return 0;
    }

    index = this._toUint32(index);
    return this._getAccumulatedValue(index);
  }

  private _getAccumulatedValue(index: number): number {
    if (index <= this.prefixSumValidIndex[0]) {
      return this.prefixSum[index];
    }

    let startIndex: number = this.prefixSumValidIndex[0] + 1;
    if (startIndex === 0) {
      this.prefixSum[0] = this.values[0];
      startIndex++;
    }

    if (index >= this.values.length) {
      index = this.values.length - 1;
    }

    for (let i: number = startIndex; i <= index; i++) {
      this.prefixSum[i] = this.prefixSum[i - 1] + this.values[i];
    }
    this.prefixSumValidIndex[0] = Math.max(this.prefixSumValidIndex[0], index);
    return this.prefixSum[index];
  }

  private _toUint32(v: number): number {
    const MAX_UINT_32: number = 4294967295;

    if (v < 0) {
      return 0;
    }

    if (v > MAX_UINT_32) {
      return MAX_UINT_32;
    }

    // tslint:disable-next-line:no-bitwise
    return v | 0;
  }
}
