export const mapObjectsToProp = (params) => {
    if (!params || params.length < 2 || !params[0]) { return; }
    const objects: any[] = params[0];
    const propRoutes: any[] = params[1].split('.');
    const getObjectProp = (object: any, props: any[]) => {
        if (props.length === 1) { return object[props[0]]; }
        const prop = props[0];
        props.shift();
        return getObjectProp(object[prop], props);
    };
    return objects.map(object => {
        return getObjectProp(object, JSON.parse(JSON.stringify(propRoutes)));
    });
};

export const handleErrorWhenLoadChoicesByUrl = (survey, options) => {
    if (!(options.serverResult !== undefined && options.serverResult.length > 0)) {
        options.question.value = undefined;
        options.question.choicesByUrl.error = undefined;
        return;
    }

    if (typeof options.serverResult !== 'string') { return; }

    try {
        const resp = JSON.parse(options.serverResult);
        if (resp.status === undefined || resp.status === null) { return; }

        if (resp.status === 204 || resp.status === 404) {
            options.question.choicesByUrl.error = undefined;
        }

        if (resp.status !== 200) {
            options.question.value = undefined;
        }
    } catch (error) {
        return;
    }
};

export const getTimeString = () => {
  return new Date().getTime().toString();
};


/**
 * Checking the end date larger than start date
 * Support for type of dateFormat(dd/mm/yy)
 * First param have to be end date, the second is start date
 */
export const compareDates = (params) => {
  if (params.length !== 2) {
      return false;
  }
  const endDateParam = params[0];
  const toDateParam = params[1];

  if (!toDateParam) {
    return true;
  }

  if (!endDateParam) {
    return false;
  }

  const pattern = /(\d{2})\/(\d{2})\/(\d{4})/;
  const endDate = new Date(endDateParam.replace(pattern, '$3-$2-$1'));
  const startDate = new Date(toDateParam.replace(pattern, '$3-$2-$1'));

  return endDate >= startDate;
};

/**
 * The conditional statement which should take 5 params.
 * 1. Value on the left of condition
 * 2. Compare operator. e.g: '=', '>', '<', '>=', '<=', 'contains', 'matchAny'
 * 3. Value on the right of condition
 * 4. Return values which should be returned if the condition matches.
 * 5. otherValues which should be returned if the condition doesn't match.
 * @param params The list of parameters.
 */
export const ifClause = (params) => {
  if (params.length !== 5) { return; }
  const leftValue = params[0];
  const operator = params[1];
  const rightValue = params[2];
  const returnValue = params[3];
  const otherValue = params[4];

  if (operator === '=') {
      if (leftValue === rightValue) { return returnValue; }
      return otherValue;
  }
  if (operator === '>') {
      if (leftValue > rightValue) { return returnValue; }
      return otherValue;
  }
  if (operator === '<') {
      if (leftValue < rightValue) { return returnValue; }
      return otherValue;
  }
  if (operator === '>=') {
      if (leftValue >= rightValue) { return returnValue; }
      return otherValue;
  }
  if (operator === '<=') {
      if (leftValue <= rightValue) { return returnValue; }
      return otherValue;
  }
  if (operator === 'contains') {
      if (leftValue.indexOf(rightValue) !== -1) { return returnValue; }
      return otherValue;
  }
  if (operator === 'matchAny') {
      if (Array.isArray(leftValue) && Array.isArray(rightValue) && leftValue.length > 0 && rightValue.length > 0) {
          const intersectItems = leftValue.filter((x) => {
              if (rightValue.indexOf(x) !== -1) { return true; }
              return false;
          });
          if (intersectItems.length > 0) { return returnValue; }
          return otherValue;
      }
      return otherValue;
  }
  return otherValue;
};

/**
 * Gets the this year.
 */
export const getThisYear = () => {
  return new Date().getFullYear();
};

/**
 * Extracts the characters from a string, between two specified indices, and returns the new sub string.
 * @param params The list of parameters: params[0] Text; params[1] Start index; params[2] Number of characters.
 */
export const subString = (params) => {
  if (!params && params.length < 3)  { return undefined; }

  const text = params[0];
  const startIndex = params[1];
  const numChars = params[2];

  return text.substring(startIndex, numChars);
};
