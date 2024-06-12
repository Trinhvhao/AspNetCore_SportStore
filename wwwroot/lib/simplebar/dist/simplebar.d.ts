export as namespace SimpleBar;
export = SimpleBar;

declare namespace SimpleBar {
    interface KnownClassNamesOptions {
        contentEl?: string;
        contentWrapper?: string;
        offset?: string;
        mask?: string;
        wrapper?: string;
        placeholder?: string;
        scrollbar?: string;
        track?: string;
        heightAutoObserverWrapperEl?: string;
        heightAutoObserverEl?: string;
        visible?: string;
        horizontal?: string;
        vertical?: string;
        hover?: string;
        dragging?: string;
    }

    type ClassNamesOptions = KnownClassNamesOptions & {
        [className: string]: string;
    };

    interface Options {
        autoHide?: boolean;
        classNames?: ClassNamesOptions;
        forceVisible?: boolean | 'x' | 'y';
        direction?: 'rtl' | 'ltr';
        timeout?: number;
        clickOnTrack?: boolean;
        scrollbarMinSize?: number;
        scrollbarMaxSize?: number;
    }
}

declare class SimpleBar {
    static instances: Pick<WeakMap<HTMLElement, SimpleBar>, 'get' | 'has'>;
    el: HTMLElement;

    constructor(element: HTMLElement, options?: SimpleBar.Options);

    static removeObserver(): void;

    recalculate(): void;

    getScrollElement(): HTMLElement;

    getContentElement(): HTMLElement;

    unMount(): void;
}