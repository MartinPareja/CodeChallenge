import {
  MAT_SELECT_CONFIG,
  MAT_SELECT_SCROLL_STRATEGY,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER_FACTORY,
  MAT_SELECT_TRIGGER,
  MatSelect,
  MatSelectChange,
  MatSelectModule,
  MatSelectTrigger
} from "./chunk-RR5EYRMP.js";
import "./chunk-PRFN55IH.js";
import {
  MatOptgroup,
  MatOption
} from "./chunk-LIYLVC2J.js";
import "./chunk-P7ZP3SAQ.js";
import "./chunk-M6NU2VD3.js";
import {
  MatError,
  MatFormField,
  MatHint,
  MatLabel,
  MatPrefix,
  MatSuffix
} from "./chunk-VP2ZGDG7.js";
import "./chunk-TJGTYFFD.js";
import "./chunk-RRUK3T6M.js";
import "./chunk-TOPIULLT.js";
import "./chunk-LHRT5ZPQ.js";
import "./chunk-OSCBKXMX.js";
import "./chunk-QCETVJKM.js";
import "./chunk-QZ54LMP6.js";
import "./chunk-DQ7OVFPD.js";
import "./chunk-N6GQAIG6.js";
import "./chunk-4SUJ3KLG.js";
import "./chunk-D5STK4MG.js";
import "./chunk-EOFW2REK.js";
import "./chunk-Q34RWIVW.js";
import "./chunk-O43THNEX.js";
import "./chunk-7OFQBS75.js";
import "./chunk-NDZIWK7R.js";
import "./chunk-N2WT6MM3.js";
import "./chunk-Z3F4H3BY.js";
import "./chunk-6PNJZOQ4.js";
import "./chunk-ZG272CAW.js";

// node_modules/@angular/material/fesm2022/select.mjs
var matSelectAnimations = {
  // Represents
  // trigger('transformPanel', [
  //   state(
  //     'void',
  //     style({
  //       opacity: 0,
  //       transform: 'scale(1, 0.8)',
  //     }),
  //   ),
  //   transition(
  //     'void => showing',
  //     animate(
  //       '120ms cubic-bezier(0, 0, 0.2, 1)',
  //       style({
  //         opacity: 1,
  //         transform: 'scale(1, 1)',
  //       }),
  //     ),
  //   ),
  //   transition('* => void', animate('100ms linear', style({opacity: 0}))),
  // ])
  /** This animation transforms the select's overlay panel on and off the page. */
  transformPanel: {
    type: 7,
    name: "transformPanel",
    definitions: [
      {
        type: 0,
        name: "void",
        styles: {
          type: 6,
          styles: { opacity: 0, transform: "scale(1, 0.8)" },
          offset: null
        }
      },
      {
        type: 1,
        expr: "void => showing",
        animation: {
          type: 4,
          styles: {
            type: 6,
            styles: { opacity: 1, transform: "scale(1, 1)" },
            offset: null
          },
          timings: "120ms cubic-bezier(0, 0, 0.2, 1)"
        },
        options: null
      },
      {
        type: 1,
        expr: "* => void",
        animation: {
          type: 4,
          styles: { type: 6, styles: { opacity: 0 }, offset: null },
          timings: "100ms linear"
        },
        options: null
      }
    ],
    options: {}
  }
};
export {
  MAT_SELECT_CONFIG,
  MAT_SELECT_SCROLL_STRATEGY,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER_FACTORY,
  MAT_SELECT_TRIGGER,
  MatError,
  MatFormField,
  MatHint,
  MatLabel,
  MatOptgroup,
  MatOption,
  MatPrefix,
  MatSelect,
  MatSelectChange,
  MatSelectModule,
  MatSelectTrigger,
  MatSuffix,
  matSelectAnimations
};
//# sourceMappingURL=@angular_material_select.js.map
